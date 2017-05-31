using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    internal class SqlTextBase : ISqlText
    {
        private string _leftToken;
        private string _rightToken;
        private string _prefix;

        public SqlTextBase(string left, string right, string prefix)
        {
            _leftToken = left;
            _rightToken = right;
            _prefix = prefix;
        }

        public virtual string Query(SqlTextEntity entity)
        {
            return string.Format(Common.QuerySQLFormatString, entity.Selection, entity.From, entity.Conditions, entity.Grouping, entity.Having, entity.OrderBy);
        }

        public virtual string QueryPage(SqlTextEntity entity)
        {
            return string.Empty;
        }

        public virtual string Insert(SqlTextEntity entity, bool key = false)
        {
            string sql = string.Format(Common.InsertSQLFormatString, entity.TableName, entity.Selection, entity.Parameter);
            return sql;
        }

        public virtual string Update(SqlTextEntity entity)
        {
            return string.Format(Common.UpdateSQLFormatString, entity.TableName, entity.Selection, entity.From, entity.Conditions);
        }

        public virtual string Delete(SqlTextEntity entity)
        {
            return string.Format(Common.DeleteSQLFormatString, entity.Selection, entity.From, entity.Conditions);
        }

        public virtual string Truncate(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return string.Empty;
            return string.Format(Common.TruncateSQLFormatString, tableName);
        }

        public virtual bool SupportParameter { get { return true; } }

        public virtual string TableName(string tableName)
        {
            return string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
        }

        public virtual string FieldName(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public virtual string FieldName(string tableName, string fieldName)
        {
            return string.Format("{0}.{1}", this.TableName(tableName), this.FieldName(fieldName));
        }

        public virtual string ParameterName(string parameterId)
        {
            return string.Format("{0}{1}", _prefix, parameterId);
        }
    }
}
