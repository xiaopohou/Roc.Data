using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roc.Data.Sql;
using System.Linq.Expressions;

namespace Roc.Data.Sql
{
    partial class SqlBuilder
    {
        private ISqlText _sql;
        private ProviderType _type;
        private SqlTextType _sqlType;
        private bool _useKey;
        private Type _tableType;
        private bool distinct;
        private bool _insertWithQuery;
        private int pageNumber;
        private int pageSize;

        public bool InsertKey { get { return _useKey; } set { _useKey = value; } }

        public bool Distinct { get { return distinct; } set { distinct = value; } }

        public SqlTextType SqlType { get { return _sqlType; } set { _sqlType = value; } }

        public int PageNumber { get { return pageNumber; } set { pageNumber = value; } }
        public int PageSize { get { return pageSize; } set { pageSize = value; } }

        #region private
        //readonly Dictionary<string, string> _tableNames = new Dictionary<string, string>();
        readonly List<SqlTableMappingEntity> _tables = new List<SqlTableMappingEntity>();
        readonly List<string> _joins = new List<string>();
        readonly List<string> _selections = new List<string>();
        readonly List<string> _conditions = new List<string>();
        readonly List<string> _sorts = new List<string>();
        readonly List<string> _groupings = new List<string>();
        readonly List<string> _havings = new List<string>();
        //readonly List<string> _splits = new List<string>();
        readonly List<string> _parameters = new List<string>();
        readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        readonly Dictionary<ExpressionType, string> _operations = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.Equal, "="},
            { ExpressionType.NotEqual, "!="},
            { ExpressionType.GreaterThan, ">"},
            { ExpressionType.LessThan, "<"},
            { ExpressionType.GreaterThanOrEqual, ">="},
            { ExpressionType.LessThanOrEqual, "<="},
            { ExpressionType.And, "AND"},
            { ExpressionType.AndAlso, "AND"},
            { ExpressionType.Or, "OR"},
            { ExpressionType.OrElse, "OR"}
        };

        readonly Dictionary<string, string> _moperations = new Dictionary<string, string>()
        {
            {"StartsWith","LIKE"},
            {"EndsWith","LIKE"},
            {"Equals","="},
            {"Contains","LIKE"}
        };

        readonly Dictionary<SqlLikeType, string> _methods = new Dictionary<SqlLikeType, string>()
        {
            {SqlLikeType.StartsWith,"LIKE"},
            {SqlLikeType.EndsWith,"LIKE"},
            {SqlLikeType.Equals,"="},
            {SqlLikeType.Contains,"LIKE"}
        };

        internal readonly Dictionary<ExpressionType, string> _noperations = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.Equal, "IS NULL"},
            { ExpressionType.NotEqual, "IS NOT NULL"}
        };

        readonly string LikeSymbol = "%";
        readonly string InSymbol = "IN";
        readonly string NotInSymbol = "NOT IN";
        readonly string NullSymbol = "NULL";

        #endregion

        public SqlBuilder(Type tableType, ProviderType type, SqlTextType sqlType = SqlTextType.Query)
        {
            _type = type;
            _sqlType = sqlType;
            _sql = GetAdapter();
            _tableType = tableType;
            _tables.Add(new SqlTableMappingEntity(tableType));
            sortType = SqlOrderByType.ASC;
            partType = SqlPartType.Where;
            pageNumber = 1;
            pageSize = 10;
        }

        public string GetSql()
        {
            string sql = string.Empty;
            SqlTextEntity entity = GetSqlEntity();
            switch (_sqlType)
            {
                case SqlTextType.Query:
                    sql = _sql.Query(entity);
                    break;
                case SqlTextType.QueryPage:
                    sql = this.GetPageSql(entity);
                    break;
                case SqlTextType.Insert:
                    sql = _sql.Insert(entity, _useKey);
                    break;
                case SqlTextType.Update:
                    sql = _sql.Update(entity);
                    break;
                case SqlTextType.Delete:
                    sql = _sql.Delete(entity);
                    break;
                case SqlTextType.Truncate:
                    string name = this.GetTableName(_tableType);
                    sql = _sql.Truncate(name);
                    break;
                default:
                    break;
            }
            return sql;
        }

        public Dictionary<string, object> GetParameters()
        {
            return parameters;
        }

        private string GetPageSql(SqlTextEntity entity)
        {
            entity.PageSize = pageSize;
            entity.PageNumber = pageNumber;
            if (_sorts.Count == 0 && _sql is Sqlserver2005)
                throw new ArgumentException("SQL Server 分页需要 ORDER BY 字段", "ORDER BY");
            return _sql.QueryPage(entity);
        }

        /// <summary>
        /// 删除语句 需要添加表名
        /// </summary>
        public void AddTableName()
        {
            _selections.Add(this.GetTableName());
        }

        public bool ContainsBinaryNodeType(ExpressionType type)
        {
            return _operations.ContainsKey(type);
        }

        public void AddTableName(Type type, string aliasName = "")
        {
            var table = _tables.FirstOrDefault(m => m.Name == type.Name);
            if (table != null)
            {
                table.AliasName = aliasName;
            }
            else
            {
                _tables.Add(new SqlTableMappingEntity(type, aliasName));
            }
        }

        public void Clear()
        {
            _sqlType = SqlTextType.Query;
            sortType = SqlOrderByType.ASC;
            partType = SqlPartType.Where;
            functionType = SqlFunctionType.NONE;
            pageNumber = 1;
            pageSize = 10;
            distinct = false;
            topString = string.Empty;
            _useKey = false;
            _insertWithQuery = false;
            _tables.Clear();
            _tables.Add(new SqlTableMappingEntity(_tableType));

            _joins.Clear();
            _conditions.Clear();
            _selections.Clear();
            _sorts.Clear();
            _groupings.Clear();
            _havings.Clear();
            _parameters.Clear();
            parameters.Clear();
        }

        #region private method
        #region 参数
        private string GetParameter(string p)
        {
            p = string.Format("{0}_{1}", p, Utils.GetUniqueIdentifier(5));
            return _sql.ParameterName(p);
        }

        private string GetParameter(string p, int index)
        {
            p = string.Format("{0}_{1}", p, index);
            return _sql.ParameterName(p);
        }

        private string GetParameter(string p, string other)
        {
            p = string.Format("{0}_{1}", p, other);
            return _sql.ParameterName(p);
        }

        private void AddParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (!parameters.ContainsKey(name))
            {
                parameters.Add(name, value);
            }
        }

        #endregion
        #region 表名 字段名

        private string GetTableName()
        {
            string name = _tableType.Name;
            if (_sqlType == SqlTextType.Insert)
            {
                return this.GetTableName(_tableType);
            }
            string aliasName = this.GetTableAliasName(name);
            if (string.IsNullOrEmpty(aliasName)) return this.GetTableName(_tableType);
            return aliasName;
        }

        private string GetTableAliasName(string name)
        {
            var table = _tables.FirstOrDefault(m => m.Name == name);
            if (table != null) return table.AliasName;
            return string.Empty;
        }

        private string GetTableName(Type type)
        {
            var table = Utils.GetTable(type);
            return _sql.TableName(table.TableName);
        }

        private string GetTableName(Type type, string aliasName)
        {
            string name = GetTableName(type);
            if (!string.IsNullOrEmpty(aliasName)) name = string.Format("{0} AS {1}", name, aliasName);
            return name;
        }

        private string GetFieldName(string tableName, string fieldName, string aliasName)
        {
            string tname = tableName;
            string fname = fieldName;
            var table = Utils.GetTable(_tableType);
            if (table != null)
            {
                tname = table.TableName;
                var column = Utils.GetColumn(table, _tableType, fieldName);
                if (column != null) fname = column.FieldName;
            }
            string result = _sql.FieldName(fname);

            string asName = GetTableAliasName(tableName);
            if (string.IsNullOrEmpty(asName)) result = _sql.FieldName(tname, fname);
            else result = string.Format("{0}.{1}", asName, result);

            if (!string.IsNullOrEmpty(aliasName) && fname != aliasName)
                result = string.Format("{0} AS {1}", result, aliasName);
            return result;
        }

        private string GetFieldName(string tableName, string fieldName)
        {
            return this.GetFieldName(tableName, fieldName, string.Empty);
        }

        private string GetFieldName(string name)
        {
            return _sql.FieldName(name);
        }
        #endregion

        private string GetFrom()
        {
            string firstTableName = _tableType.Name;
            string asName = this.GetTableAliasName(firstTableName);
            string name = GetTableName(_tableType, asName);
            if (_joins.Count > 0)
                return string.Format("{0} {1}", name, string.Join(" ", _joins));
            return name;
        }

        private string GetSelection()
        {
            string section = string.Empty;
            if (_sqlType == SqlTextType.Query || _sqlType == SqlTextType.QueryPage)
            {
                if (_selections.Count == 0)
                {
                    string aliasName = this.GetTableAliasName(_tableType.Name);
                    if (string.IsNullOrEmpty(aliasName)) aliasName = GetTableName(_tableType);
                    section = string.Format("{0}.*", aliasName);
                    if (!string.IsNullOrEmpty(topString)) section = string.Format("{0} {1}", topString, section);
                }
                else if (!string.IsNullOrEmpty(topString))
                    section = string.Format("{0} {1}", topString, string.Join(",", _selections));
                else section = string.Join(",", _selections);
                if (distinct) section = string.Format("DISTINCT {0}", section);
            }
            else if (_sqlType == SqlTextType.Delete)
                section = GetForamtList(" ", string.Empty, _selections);
            else if (_sqlType == SqlTextType.Update || _sqlType == SqlTextType.Insert)
                section = GetForamtList(",", string.Empty, _selections);

            distinct = false;
            topString = string.Empty;
            return section;
        }

        private string GetForamtList(string join, string head, List<string> list)
        {
            if (list.Count > 0) return head + string.Join(join, list);
            return string.Empty;
        }

        private string GetParameter()
        {
            if (_insertWithQuery) return _parameters.FirstOrDefault();
            return GetForamtList(",", "VALUES ", _parameters);
        }

        private SqlTextEntity GetSqlEntity()
        {
            SqlTextEntity entity = new SqlTextEntity();
            entity.Having = GetForamtList("", "HAVING ", _havings);
            entity.Grouping = GetForamtList(",", "GROUP BY ", _groupings);
            entity.OrderBy = GetForamtList(",", "ORDER BY ", _sorts);
            entity.Conditions = GetForamtList("", "WHERE ", _conditions);
            entity.Parameter = GetParameter();
            entity.From = GetFrom();
            entity.Selection = GetSelection();
            entity.TableName = GetTableName();
            return entity;
        }

        private ISqlText GetAdapter()
        {
            switch (_type)
            {
                case ProviderType.SQLServer2005:
                    return new Sqlserver2005();
                case ProviderType.SQLServer2012:
                    return new Sqlserver2012();
                case ProviderType.SQLite:
                    return new Sqlite3();
                case ProviderType.Oracle:
                    return new Oracle();
                case ProviderType.MySql:
                    return new Mysql();
                default:
                    throw new ArgumentException("还没有实现该数据库适配");
            }
        }

        #endregion
    }
}
