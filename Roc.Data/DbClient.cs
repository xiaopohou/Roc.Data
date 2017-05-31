using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Roc.Data.Core;
using System.Collections;

namespace Roc.Data
{
    public class DbClient : IDisposable
    {
        DbHelper db;
        SqlEntity sql;

        public DbClient()
        {
            this.Init();
        }

        public DbClient(ProviderType type)
        {
            this.Type = type;
            this.Init();
        }

        public ProviderType Type { get; set; }
        /// <summary>
        /// 统一设置执行时间
        /// </summary>
        public int CommandTimeout
        {
            get;
            set;
        }

        public Action<SqlEntity> Log
        {
            get;
            set;
        }

        public Action<SqlEntity, Exception> Error
        {
            get;
            set;
        }

        private void Init()
        {
            db = new DbHelper();
        }

        public void UseTran()
        {
            //db.UseTran((d, idb) =>
            //{

            //});
        }

        public object ExecuteScalar(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.ExecuteScalar();
        }

        public T ExecuteScalar<T>(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            object value = this.ExecuteScalar(sqlText, obj, type);
            if (value == null || value == DBNull.Value) return default(T);
            return (T)value;
        }

        public int ExecuteNonQuery(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.ExecuteNonQuery();
        }

        /// <summary>
        ///返回DataReader
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.ExecuteReader();
        }

        public IEnumerable<T> GetList<T>(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.GetList<T>();
        }

        public GridReader GetReader(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.GetReader();
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            PrepareSql(sqlText, obj, type);
            return db.ExecuteDataSet();
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            var ds = this.ExecuteDataSet(sqlText, obj, type);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        private void PrepareSql(string sqlText, object obj, CommandType type = CommandType.Text)
        {
            sql = new SqlEntity(sqlText, type: type);
            sql.Type = Type;
            sql.OnDbBefore = Log;
            sql.OnDbError = Error;
            sql.CommandTimeout = CommandTimeout;
            if (obj == null) return;
            if (obj is SqlParameterEntity)
            {
                sql.AddParameter(obj as SqlParameterEntity);
            }
            else if (obj is IDictionary<string, object>)
            {
                sql.AddParameters(obj as IDictionary<string, object>);
            }
            else if (obj is IEnumerable)
            {
                Type objType = obj.GetType();
                bool isSqlObj = objType.GetGenericArguments().Any(m => m == typeof(SqlParameterEntity));
                if (isSqlObj)
                {
                    sql.AddParameters(obj as IEnumerable<SqlParameterEntity>);
                }
            }
            else
            {
                sql.AddParameter(obj);
            }
            db.UpdateSqlEntity(sql);
        }

        public void Dispose()
        {
            sql = null;
            db.Dispose();
        }
    }
}
