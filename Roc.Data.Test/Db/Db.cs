using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Test
{
    public class Db : ITest
    {
        public void Do(ProviderType type)
        {
            SqlLam<Test> sql = new SqlLam<Test>("u", type);
            //sql.Top(10);

            DbClient db = new DbClient();
            db.Error = (s, e) =>
            {
                Console.WriteLine(e.Message);
            };
            db.Log = (s) =>
            {
                Log.WriteLog(1, "Insert_Log", s.Text);
            };
            //var list = db.GetList<Test>(sql.GetSql(), sql.GetParameters());

            //foreach (var item in list)
            //{
            //    Console.WriteLine(string.Format("id: {0}, int 16: {1} , double: {2}, datetime: {3} , guid: {4}, string: {5}", item.Id, item.F_Int16, item.F_Double, item.F_DateTime, item.F_Guid, item.F_String));
            //}
            DateTime begin = DateTime.Now;
            Console.WriteLine(string.Format("当前时间: {0}", begin));

            //遍历入库
            //for (int i = 0; i < 10000; i++)
            //{
            //    sql.Clear();
            //    sql.Insert(new Test());
            //    string insertSql = sql.GetSql();
            //    int count = db.ExecuteNonQuery(insertSql, sql.GetParameters());
            //}
            //一次性入库
            sql.Clear();
            List<Test> list = new List<Test>(100);
            for (int i = 0; i < 200; i++)
            {
                list.Add(new Test());
            }
            sql.Insert(list);

            string insertSql = sql.GetSql();
            int count = db.ExecuteNonQuery(insertSql, sql.GetParameters());

            TimeSpan ts = DateTime.Now - begin;
            Console.WriteLine(string.Format("共用时: {0}", ts.TotalMilliseconds));
        }
    }
}
