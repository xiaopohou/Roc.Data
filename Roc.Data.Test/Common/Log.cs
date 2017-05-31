using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class Log
    {
        public static void WriteLog(int no, string fileName, string desc, string expression, SqlLamBase sql)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("序号: [{0}]", no).AppendLine();
            sb.AppendFormat("描述: [{0}]", string.IsNullOrEmpty(desc) ? "无" : desc).AppendLine();
            sb.AppendFormat("表达式: {0}", string.IsNullOrEmpty(expression) ? "无" : expression).AppendLine();
            sb.AppendFormat("SQL: {0}", sql.GetSql()).AppendLine();
            var parameters = sql.GetParameters();
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    sb.AppendFormat("参数: Key: [{0}], Value: [{1}]", item.Key, item.Value).AppendLine();
                }
            }
            else
            {
                sb.AppendFormat("参数: [无]").AppendLine();
            }
            LogHelper.WriteLog(fileName, sb.ToString());
        }

        public static void WriteLog(int no, string fileName, string desc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("序号: [{0}]", no).AppendLine();
            sb.AppendFormat("描述: [{0}]", string.IsNullOrEmpty(desc) ? "无" : desc).AppendLine();

            LogHelper.WriteLog(fileName, sb.ToString());
        }
    }
}
