using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    class Sqlserver2012 : SqlTextBase, ISqlText
    {
        public Sqlserver2012()
            : base(Common.LeftTokens[0], Common.RightTokens[0], Common.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlTextEntity entity)
        {
            int offset = (entity.PageNumber - 1) * entity.PageSize;

            string template = @"SELECT {0} FROM {1} {2} {3} OFFSET {4} ROWS
FETCH NEXT {5} ROWS ONLY";
            return string.Format(template, entity.Selection, entity.From, entity.Conditions, entity.OrderBy, offset, entity.PageSize);
        }

        public override string Insert(SqlTextEntity entity, bool key = false)
        {
            string sql = base.Insert(entity, key);
            if (key) sql = string.Format("{0};{1}", sql, Common.SqlserverAutoKeySQLString);
            return sql;
        }
    }
}
