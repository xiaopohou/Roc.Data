using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    internal class Oracle : SqlTextBase, ISqlText
    {
        public Oracle()
            : base(Common.LeftTokens[2], Common.RightTokens[2], Common.ParamPrefixs[2])
        {

        }

        public override string QueryPage(SqlTextEntity entity)
        {
            int pageSize = entity.PageSize;
            int begin = (entity.PageNumber - 1) * pageSize;
            int end = entity.PageNumber * pageSize;
            return string.Format(@"SELECT * FROM (
SELECT A.*, ROWNUM RN FROM (SELECT {0} FROM {1} {2} {3}) A WHERE ROWNUM <= {5})WHERE RN >{4}", entity.Selection, entity.From, entity.Conditions, entity.OrderBy, begin, end);
        }
    }
}
