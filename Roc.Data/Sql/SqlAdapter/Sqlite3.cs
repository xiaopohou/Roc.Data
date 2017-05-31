using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    internal class Sqlite3 : SqlTextBase, ISqlText
    {
        public Sqlite3()
            : base(Common.LeftTokens[0], Common.RightTokens[0], Common.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlTextEntity entity)
        {
            int limit = entity.PageSize;
            int offset = limit * (entity.PageNumber - 1);
            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4} OFFSET {5}", entity.Selection, entity.From, entity.Conditions, entity.OrderBy, limit, offset);
        }

        public override string FieldName(string tableName, string fieldName)
        {
            return this.FieldName(fieldName);
        }
    }
}
