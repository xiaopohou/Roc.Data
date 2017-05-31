using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlEntity
    {
        /// <summary>
        /// SQL 文本内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 清空参数
        /// </summary>
        public bool AutoClearParameters { get; set; }
        /// <summary>
        ///参数
        /// </summary>
        public List<SqlParameterEntity> Parameters { get; set; }
        /// <summary>
        /// CommandType
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。单位 秒 默认 60
        /// </summary>
        public int CommandTimeout { get; set; }

        public Action<SqlEntity> OnDbBefore { get; set; }

        public Action<SqlEntity, Exception> OnDbError { get; set; }

        public string ConnectionString { get; set; }

        public ProviderType Type { get; set; }

        public string ProviderName { get; set; }
        /// <summary>
        /// 默认连接字符串 KEY
        /// </summary>
        public string ConnectionKey = "connectString";
        public readonly string DefaultProviderName = "System.Data.SqlClient";

        #region 构造函数

        public SqlEntity(string text, IEnumerable<SqlParameterEntity> parameters = null, CommandType type = CommandType.Text, int timeout = 60)
        {
            this.Init();
            this.Text = text;
            this.CommandType = type;
            this.AutoClearParameters = true;
            this.Parameters = new List<SqlParameterEntity>();
            if (parameters != null && parameters.Count() > 0)
                this.Parameters.AddRange(parameters);
            this.CommandTimeout = timeout;
        }
        #endregion

        private void Init()
        {
            var manager = ConfigurationManager.ConnectionStrings[ConnectionKey];
            if (manager != null)
            {
                this.ConnectionString = manager.ConnectionString;
                this.ProviderName = manager.ProviderName;
            }
            if (string.IsNullOrEmpty(this.ProviderName))
            {
                this.ProviderName = DefaultProviderName;
            }

            if (string.IsNullOrEmpty(this.ConnectionString))
                throw new ArgumentNullException("ConnectionString", "连接字符串为空");

            this.CheckProvider();
        }

        private void CheckProvider()
        {
            ProviderType type = ProviderType.SQLServer2005 | ProviderType.SQLServer2012;
            string providerName = this.ProviderName;
            if (providerName == DefaultProviderName)
            {
                this.Type = type;
                return;
            }
            DbDriver driver = GlobalConfig.GetDriver(ProviderType.None, providerName);
            if (driver != null)
            {
                this.Type = driver.Type;
                return;
            }
            throw new ArgumentNullException("DbDriver", string.Format("[{0}]--没有找到对应的数据驱动", providerName));
        }

        public IDbConnection GetConnection()
        {
            //var cs = DbProviderFactories.GetFactoryClasses();
            var factory = DbProviderFactories.GetFactory(this.ProviderName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = this.ConnectionString;
            return connection;
        }

        private IEnumerable<IDbDataParameter> CreateParameters(Dictionary<string, object> dic)
        {
            if (dic == null || dic.Count < 1) return null;
            var factory = DbProviderFactories.GetFactory(this.ProviderName);
            List<IDbDataParameter> list = new List<IDbDataParameter>();
            foreach (var item in dic)
            {
                var p = factory.CreateParameter();
                p.ParameterName = item.Key;
                p.Value = item.Value;
                list.Add(p);
            }
            return list;
        }

        public void AddParameter(string name, string value, ParameterDirection direction = ParameterDirection.Input)
        {
            this.RemoveParameters(name);
            this.Parameters.Add(new SqlParameterEntity(name, value, direction));
        }

        public void AddParameter(SqlParameterEntity p)
        {
            this.RemoveParameters(p.Name);
            this.Parameters.Add(p);
        }

        public void AddParameter(object obj)
        {
            if (obj == null) return;
            Type type = obj.GetType();
            var ps = Utils.GetPropertyInfos(type);
            if (ps != null && ps.Length > 0)
            {
                foreach (var item in ps)
                {
                    this.RemoveParameters(item.Name);
                    var p = type.GetProperty(item.Name);
                    object value = p.GetValue(obj, null);
                    this.Parameters.Add(new SqlParameterEntity(item.Name, value));
                }
            }
        }

        public void AddParameter<T>(T obj, bool ignoreKey = true)
        {
            if (obj == null) return;
            var ps = Utils.GetPropertyInfos<T>(ignoreKey, true);
            if (ps != null && ps.Count() > 0)
            {
                foreach (var item in ps)
                {
                    this.RemoveParameters(item.Name);
                    var p = typeof(T).GetProperty(item.Name);
                    object value = p.GetValue(obj, null);
                    this.Parameters.Add(new SqlParameterEntity(item.Name, value));
                }
            }
        }

        public void AddParameters(IDictionary<string, object> dic)
        {
            if (dic == null || dic.Count < 1) return;
            foreach (var item in dic)
            {
                this.RemoveParameters(item.Key);
                this.Parameters.Add(new SqlParameterEntity(item.Key, item.Value));
            }
        }

        public void AddParameters(IEnumerable<SqlParameterEntity> ps)
        {
            if (ps == null || ps.Count() < 1) return;

            foreach (var item in ps)
            {
                this.RemoveParameters(item.Name);
                this.Parameters.Add(item);
            }
        }

        public void ClearParameters()
        {
            this.Parameters.Clear();
        }

        private void RemoveParameters(string name)
        {
            this.Parameters.RemoveAll(m => m.Name == name);
        }

        public T Get<T>(string key)
        {
            var p = this.Parameters.FirstOrDefault(m => m.Name == key);
            if (p != null)
            {
                var value = p.AttachedParam.Value;
                if (value == DBNull.Value) return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }
    }
}
