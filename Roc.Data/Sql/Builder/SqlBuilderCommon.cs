using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Sql
{
    internal partial class SqlBuilder
    {
        public void BeginExpression()
        {
            this.AppendCondition("(");
        }

        public void EndExpression()
        {
            this.AppendCondition(")");
        }

        public void And()
        {
            if (HasCondition()) this.AppendCondition(" AND ");
        }

        public void Or()
        {
            if (HasCondition()) this.AppendCondition(" OR ");
        }

        public void Not()
        {
            this.AppendCondition(" NOT ");
        }

        public void AppendCondition(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                if (partType == SqlPartType.Where) _conditions.Add(condition);
                else if (partType == SqlPartType.Having) _havings.Add(condition);
            }
        }

        private bool HasCondition()
        {
            bool flag = false;
            switch (partType)
            {
                case SqlPartType.Where:
                    flag = _conditions.Count > 0;
                    break;
                case SqlPartType.Having:
                    flag = _havings.Count > 0;
                    break;
            }
            return flag;
        }
    }
}
