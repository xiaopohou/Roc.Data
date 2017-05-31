using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    class Sqlserver2005 : SqlTextBase, ISqlText
    {
        public Sqlserver2005()
            : base(Common.LeftTokens[0], Common.RightTokens[0], Common.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlTextEntity entity)
        {
            int pageSize = entity.PageSize;
            if (entity.PageNumber < 2)
            {
                return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}", entity.Selection, entity.From, entity.Conditions, entity.OrderBy, pageSize);
            }

            string innerQuery = string.Format("SELECT {0},ROW_NUMBER() OVER ({1}) AS RN FROM {2} {3}", entity.Selection, entity.OrderBy, entity.From, entity.Conditions);
            return string.Format("SELECT TOP {0} * FROM ({1}) InnerQuery WHERE RN > {2} ORDER BY RN", pageSize, innerQuery, pageSize * (entity.PageNumber - 1));
        }

        public override string Insert(SqlTextEntity entity, bool key = false)
        {
            string sql = base.Insert(entity, key);
            if (key) sql = string.Format("{0};{1}", sql, Common.SqlserverAutoKeySQLString);
            return sql;
        }
    }
}
