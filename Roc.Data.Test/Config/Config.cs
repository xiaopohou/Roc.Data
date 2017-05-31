using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Roc.Data.Test.Model;

namespace Roc.Data.Test.Config
{
    class Config : ITest
    {
        public void Do(ProviderType type)
        {
            string fileName = "Config";
            int count = 1;
            SqlLam<Area> sql = new SqlLam<Area>("a", type);

            Log.WriteLog(count, fileName, "特性配置表名", "SqlLam<Area> sql = new SqlLam<Area>(\"a\", type);", sql);

            //sql.Clear();
            count++;
            //SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), "T_Area_全局");
            //GlobalConfig.AddTable(areaTable);
            Log.WriteLog(count, fileName, "全局配置表名", "SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), \"T_Area\");GlobalConfig.Tables.Add(areaTable);", sql);

            //sql.Clear();
            count++;
            Log.WriteLog(count, fileName, "全局配置表名两种方式都存在", "SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), \"T_Area\");GlobalConfig.Tables.Add(areaTable);", sql);

            sql.Clear();
            count++;
            sql.Select(m => m.AreaName);
            Log.WriteLog(count, fileName, "特性配置列名", "SqlLam<Area> sql = new SqlLam<Area>(\"a\", type);sql.Select(m => m.AreaName);", sql);

            sql.Clear();
            count++;
            //areaTable.AddColumn(new SqlColumnEntity("AreaName", "F_AreaName"));
            //GlobalConfig.AddTable(areaTable);
            sql.Select(m => m.AreaName);
            Log.WriteLog(count, fileName, "全局配置列名", "areaTable.Columns.Add(new SqlColumnEntity(\"AreaName\", \"F_AreaName\"));", sql);


            sql.Clear();
            count++;
            sql.Select(m => new { m.AreaName, m.Layer });
            Log.WriteLog(count, fileName, "忽略查询列名", " sql.Select(m => new { m.AreaName, m.Layer });", sql);

            sql.Clear();
            count++;
            Area a = new Area();
            a.Layer = 10;
            a.AreaName = "测试";
            sql.Insert(a, false);
            Log.WriteLog(count, fileName, "忽略插入列名", " sql.Insert(a);", sql);

            sql.Clear();
            count++;

            a.Layer = 10;
            a.AreaName = "测试";
            sql.Update(a, false).Where(m => m.Id);
            Log.WriteLog(count, fileName, "忽略修改列名", " sql.Update(a, false);", sql);

            sql.Clear();
            count++;

            a.Layer = 10;
            a.AreaName = "测试";
            sql.Update(a, m => new { m.ParentId, m.Id, m.AreaName }).Where(m => m.Id);
            Log.WriteLog(count, fileName, "忽略修改列名部分字段", " sql.Update(a, m => new { m.ParentId, m.Id, m.AreaName }).Where(m => m.Id);", sql);
        }
    }
}
