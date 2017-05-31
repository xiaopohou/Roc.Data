using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roc.Data;
using Roc.Data.Test.Model;

namespace Roc.Data.Test.Sql
{
    public class Query : ITest
    {
        public void Do(ProviderType type)
        {
            string fileName = "Query";
            int count = 1;
            SqlLam<Area> sql = new SqlLam<Area>("u", type);
            sql.Type = ProviderType.Oracle;

            GlobalConfig.UseDb(ProviderType.Oracle);

            Log.WriteLog(count, fileName, "生成最简单的SQL", "SqlLam<Area> sql = new SqlLam<Area>();", sql);

            count++;
            sql = new SqlLam<Area>("u");
            Log.WriteLog(count, fileName, "带别名的简单SQL", "SqlLam<Area> sql = new SqlLam<Area>(\"u\");", sql);

            count++;
            sql.As("u");
            Log.WriteLog(count, fileName, "带别名的简单SQL2,和上面效果一样", "SqlLam<Area> sql = new SqlLam<Area>();\r\nsql.As(\"u\");", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Top(100);
            Log.WriteLog(count, fileName, "SQL TOP * ", "sql.Top(100);", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Top(100, true);
            Log.WriteLog(count, fileName, "SQL TOP * 带 percent ", "sql.Top(100, true);", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Select(m => m.AreaCode);
            Log.WriteLog(count, fileName, "SQL Select 只查一列 ", "sql.Select(m => m.F_CreatorUserId);", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Select(m => new { m.AreaCode, m.AreaId, m.AreaName });
            Log.WriteLog(count, fileName, "SQL Select 查多列 ", "sql.Select(m => m.F_CreatorUserId);", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Top(100).Select(m => new { m.AreaCode, m.AreaId, m.AreaName });
            Log.WriteLog(count, fileName, "SQL TOP 其他列 先写 TOP 再写 Select", "sql.Top(100).Select(m => new { m.F_CreatorUserId, m.F_DeleteMark, m.F_EnCode });", sql);

            count++;
            sql = new SqlLam<Area>();
            sql.Select(m => new { m.AreaCode, m.AreaId, m.AreaName }).Top(100);
            Log.WriteLog(count, fileName, "SQL TOP 其他列 先写 Select 再写 TOP, 其结果一样", "sql.Top(100).Select(m => new { m.F_CreatorUserId, m.F_DeleteMark, m.F_EnCode });", sql);

            count++;
            sql.Clear();
            sql.As("a");
            sql.Where(m => !string.IsNullOrEmpty(m.AreaCode));
            Log.WriteLog(count, fileName, "SQL 实现 IsNullOrEmpty 方法", "sql.Where(m => string.IsNullOrEmpty(m.AreaCode));", sql);
        }
    }
}
