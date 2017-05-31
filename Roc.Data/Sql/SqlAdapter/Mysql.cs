using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    internal class Mysql : SqlTextBase, ISqlText
    {
        public Mysql()
            : base(Common.LeftTokens[1], Common.RightTokens[1], Common.ParamPrefixs[1])
        {

        }

        public override string QueryPage(SqlTextEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);

            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4},{5}", entity.Selection, entity.From, entity.Conditions, entity.OrderBy, limit, pageSize);
        }
    }
}
