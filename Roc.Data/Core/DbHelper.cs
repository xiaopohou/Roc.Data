using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Roc.Data.Core
{
    public class DbHelper : IDisposable
    {
        private IDbConnection conn;
        private IDbTransaction tran;
        private SqlEntity sql;
        private string prefix;
        private bool close;

        public DbHelper() { }

        public DbHelper(SqlEntity sql)
        {
            this.UpdateSqlEntity(sql);
        }

        public bool AutoCloseConnection { get { return close; } set { close = value; } }

        public void UpdateSqlEntity(SqlEntity sql)
        {
            this.sql = sql;
            this.close = true;
            this.prefix = Common.GetParamPrefixs(sql.Type);
        }
        /// <summary>
        /// 准备Command
        /// </summary>
        /// <param name="cmd"></param>
        private void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandType = sql.CommandType;
            cmd.CommandText = sql.Text;
            cmd.CommandTimeout = sql.CommandTimeout;
            var dbs = sql.Parameters;
            cmd.Parameters.Clear();
            if (dbs != null && dbs.Count > 0)
            {
                foreach (var item in dbs)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = GetParameterName(item.Name);
                    p.Value = item.Value;
                    p.Direction = item.Direction;
                    if (item.Size.HasValue) p.Size = item.Size.Value;
                    if (item.DbType.HasValue) p.DbType = item.DbType.Value;
                    cmd.Parameters.Add(p);
                    item.AttachedParam = p;
                }
            }
            if (sql.OnDbBefore != null) sql.OnDbBefore(sql);
        }

        private void AfterCommand(IDbCommand cmd)
        {
            if (sql.AutoClearParameters)
                cmd.Parameters.Clear();
        }

        private string GetParameterName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            if (prefix == name[0].ToString()) return name;
            return prefix + name;
        }

        private void PrepareConnection()
        {
            if (conn == null) conn = sql.GetConnection();
            else if (string.IsNullOrEmpty(conn.ConnectionString))
                conn.ConnectionString = sql.ConnectionString;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        public void BeginTran(IsolationLevel iso = IsolationLevel.Unspecified)
        {
            tran = conn.BeginTransaction(iso);
        }

        public void Commit()
        {
            if (tran != null) tran.Commit();
        }

        public void Rollback()
        {
            if (tran != null) tran.Rollback();
        }

        public void UseTran(Action<DbHelper, IDbConnection> action)
        {
            close = false;
            sql.AutoClearParameters = false;
            PrepareConnection();
            try
            {
                this.BeginTran();
                if (action != null)
                    action(this, conn);
                this.Commit();
            }
            catch (Exception e)
            {
                this.Rollback();
                OnError(e);
            }
            finally
            {
                close = true;
                sql.AutoClearParameters = true;
                this.Dispose();
            }
        }

        /// <summary>
        /// 单行单列
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            PrepareConnection();
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    PrepareCommand(cmd);
                    var obj = cmd.ExecuteScalar();
                    AfterCommand(cmd);
                    return obj;
                }
                catch (Exception e)
                {
                    OnError(e);
                    return null;
                }
                finally
                {
                    this.Dispose();
                }
            }
        }

        public T ExecuteScalar<T>()
        {
            object obj = ExecuteScalar();
            if (obj != null && obj != DBNull.Value)
                return (T)Convert.ChangeType(obj, typeof(T));
            return default(T);
        }
        /// <summary>
        /// 受影响的行数
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            PrepareConnection();
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    PrepareCommand(cmd);
                    int count = cmd.ExecuteNonQuery();
                    AfterCommand(cmd);
                    return count;
                }
                catch (Exception e)
                {
                    OnError(e);
                    Rollback();
                    return -1;
                }
                finally
                {
                    this.Dispose();
                }
            }
        }

        /// <summary>
        ///返回DataReader
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        {
            PrepareConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    PrepareCommand(cmd);
                    var reader = cmd.ExecuteReader(behavior);
                    AfterCommand(cmd);
                    return reader;
                }
            }
            catch (Exception e)
            {
                OnError(e);
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }

        public IEnumerable<T> GetList<T>()
        {
            var reader = ExecuteReader(CommandBehavior.CloseConnection);
            GridReader gr = new GridReader(reader);
            return gr.Read<T>();
        }

        public GridReader GetReader()
        {
            return new GridReader(ExecuteReader(CommandBehavior.CloseConnection));
        }
        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(sql.ProviderName);
                var adapter = factory.CreateDataAdapter();
                using (var conn = factory.CreateConnection())
                {
                    conn.ConnectionString = sql.ConnectionString;
                    using (var cmd = conn.CreateCommand())
                    {
                        PrepareCommand(cmd);
                        PrepareConnection();
                        adapter.SelectCommand = cmd;

                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        AfterCommand(cmd);
                        return ds;
                    }
                }
            }
            catch (Exception e)
            {
                OnError(e);
                return null;
            }
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteDataTable()
        {
            DataSet ds = ExecuteDataSet();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        private void OnError(Exception e)
        {
            if (sql.OnDbError != null) sql.OnDbError(sql, e);
        }

        public void Dispose()
        {
            if (close)
            {
                if (tran != null)
                {
                    tran.Commit();
                    tran = null;
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}
