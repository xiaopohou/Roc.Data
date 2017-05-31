using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using Roc.Data.Core;
using Roc.Data;
using System.Configuration;
using Roc.Data.Test.Sql;
using Roc.Data.Test.Config;
using Roc.Data.Test2;

namespace Roc.Data.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<ITest> list = new List<ITest>();
            list.Add(new Query());
            list.Add(new Config.Config());
            list.Add(new Db());
            foreach (var item in list)
            {
                item.Do(ProviderType.SQLServer2005);
            }
           
            //SqlTableEntity areaTable = new SqlTableEntity(typeof(Area), "T_Area");
            //areaTable.Columns.Add(new SqlColumnEntity("AreaId", "C_AreaId"));
            //GlobalConfig.Tables.Add(areaTable);

            Console.Read();
        }

        static void Main7(string[] args)
        {
            // IDictionary<string, object> dic = new Dictionary<string, object>();

            // Hashtable dic = new Hashtable();

            //List<SqlParameterEntity> list = new List<SqlParameterEntity>();
            List<object> list = new List<object>();

            //ArrayList dic = new ArrayList();

            var dic = new { A = 1, B = 2 };

            bool flag = dic.GetType().GetInterfaces().Any(m => m == typeof(IDictionary));
            var type = list.GetType();
            var type2 = type.GetGenericTypeDefinition();

            Console.WriteLine(type.FullName);
            Console.WriteLine(type2.FullName);

            foreach (var item in type.GetGenericArguments())
            {
                Console.WriteLine(item.FullName);
            }

            Console.WriteLine(flag.ToString());

            Console.Read();
        }

        static void Main2(string[] args)
        {
            //List<SqlColumnEntity> userColumns = new List<SqlColumnEntity>();
            //userColumns.Add(new SqlColumnEntity("age", "t_age"));
            GlobalConfig.AddTable(new SqlTableEntity(typeof(User), "Sys_User"));
            GlobalConfig.AddTable(new SqlTableEntity(typeof(Area), "Sys_Area"));

            List<SqlColumnEntity> testColumns = new List<SqlColumnEntity>();
            testColumns.Add(new SqlColumnEntity("Other") { Ignore = true });
            //testColumns.Add(new SqlColumnEntity("Id") { Key = true });
            GlobalConfig.AddTable(new SqlTableEntity(typeof(Test2)) { });

            GlobalConfig.UseDb(ProviderType.MySql);

            //var sql = new SqlLam<Area>().As("a");
            //sql = sql.GroupBy(m => m.F_Layers).Select(m => new { Layers = m.F_Layers }).Count(m => new { Count = m.F_Id }).Having().OrderByDescending(m => m.F_Layers);
            //SqlLam<User> sql = new SqlLam<User>().As("u");

            //sql = sql.Distinct(m => m.F_Layers).Select(m => new { A = m.F_ParentId });
            //var sql = new SqlLam<User>().As("u").Where(m => m.F_Account == "abc").Select(m => m.F_Account, m => m.F_Birthday);

            //var sqlJoin = sql.Join<UserLogon>((a, u) => a.F_Id == u.F_UserId && a.F_Account == u.F_AnswerQuestion, aliasName: "ul").Where(m => m.F_Language == "cc").Select(m => m.F_LockEndDate, m => m.F_LogOnCount);

            //sql = sql.Select(m => m.F_CreatorTime);

            //sqlJoin.Join<Area>((u, a) => u.F_Id == a.F_Id, SqlJoinType.RIGHT, aliasName: "a");

            //sql.OrderBy<string>(m => m.F_DepartmentId, m => m.F_Id);

            //var sql = new SqlLam<UserLogon>("ul");

            //sql.Having(m => !m.F_MultiUserLogin, m => m.F_LogOnCount > 1).OrderBy(m => m.F_LogOnCount);

            //sql.Where(m => m.F_MultiUserLogin);

            DateTime dt = DateTime.Now;

            //sql.Select(m => m.F_LogOnCount).SelectDistinct(m => m.F_LastVisitTime)
            //   .Where(m => m.F_LogOnCount > 100 || m.F_AllowStartTime == dt).And(m => m.F_Language == "abc" || m.F_Id == "a")
            //    .GroupBy(m => m.F_UserId, m => m.F_Theme).SelectFunction(SqlFunctionType.AVG, m => m.F_LogOnCount).OrderBy(m => m.F_Question);

            //sql.Where(m => m.F_LogOnCount > 100).Begin().And(m => m.F_Language == "abc").Or(m => m.F_AnswerQuestion == "c").End()
            //   .GroupBy(m => m.F_UserId).OrderBy(m => m.F_Question);

            #region join
            //string[] array = new string[] { "a", "b", "c" };
            //var sql = new SqlLam<User>("u");
            //sql.Join<UserLogon>((u, ul) => u.F_Id == ul.F_UserId, aliasName: "ul").Where(m => m.F_UserId == "abc");
            //sql.Where(m => array.Contains(m.F_HeadIcon));
            //sql.In<string>(m => m.F_Language, array).NotIn(m => m.F_Question, array, false);
            //sql.Where(m => array.Contains(m.F_AnswerQuestion) == !m.F_MultiUserLogin);
            #endregion

            #region where in sql
            //var sqlUser = new SqlLam<User>("u");
            //sqlUser.Select(m => m.F_Id).Where(m => m.F_NickName == "test");

            //var sql = new SqlLam<UserLogon>("ul");
            //sql.In(m => m.F_UserId, sqlUser).And(m => m.F_UserOnLine);
            #endregion

            #region delete
            //var sqlUser = new SqlLam<User>("u");
            //sqlUser.Select(m => m.F_Id).Where(m => m.F_NickName == "test");

            //var sql = new SqlLam<UserLogon>("ul");
            //sql.Delete().In(m => m.F_UserId, sqlUser, false).And(m => m.F_Theme == "bb");

            //sql.Truncate();
            //sql.Update(m => new { m.Id, m.Name }, null, new { Id = 2 });
            #endregion

            #region update

            Test2 model = new Test2();
            model.Id = 2;
            model.Name = "Roc";
            model.FullName = "Chengpeng333444";

            var sql = new SqlLam<Test2>("t");
            //sql.Update(new { }, m => m.Name).Where(m => m.Id == 1);
            //sql.Update(model, m => m.Name).Where(m => m.Id == model.Id);
            //sql.Update(model).Where(m => m.Id == model.Id);
            //sql.Update(model, m => m.FullName, m => m.Name).Where(m => m.Id == model.Id);
            //sql.Update(model, m => new { m.FullName, m.Name, m.AddTime }).Where(m => m.Id == model.Id);
            //User user = new User();
            //user.Id = 1;
            //user.Name = "www";
            //user.F_WeChat = "微信";
            ////sql.Update(new { Id = 1, Name = "aaa" }).Where(m => m.Id);
            //sql.Update(user).Where(m => m.Id);
            #endregion

            string sqlString = sql.GetSql();

            Console.WriteLine(string.Format("SQL: {0}", sqlString));
            var ps = sql.GetParameters();
            foreach (var item in ps)
            {
                Console.WriteLine(string.Format("Key: {0}, Value: {1}", item.Key, item.Value));
            }

            int count = ExecuteNoQuery(sqlString, ps);//insert update delete 使用

            Console.WriteLine(string.Format("受影响的行数: {0}", count));

            //测试字典
            Console.Read();
        }

        static void Main1(string[] args)
        {
            //GlobalConfig.UseDb(ProviderType.MySql);

            SqlLam<User> sql = new SqlLam<User>("u");
            sql.Select(m => new { m.F_Account, m.F_Birthday });
            //sql.Where(m => m.F_CreatorTime == DateTime.Now || m.F_DepartmentId == "aaa");
            ////sql.And(m => m.F_DeleteUserId == "a");
            sql.And().Begin();
            sql.Or(m => m.F_Description == "bbb");
            sql.Or(m => m.F_DutyId == "333");
            sql.End();
            //sql.Select(m => m.F_SecurityLevel, m => m.F_RoleId).GroupBy(m => m.F_NickName);
            //sql.Where(m => m.F_RoleId == "a").OrderBy(m => m.Id);
            //sql.Delete(m => m.Id == 1);
            //sql.Truncate();// 有问题
            int age = 10;

            UserLogon ul = new UserLogon();
            ul.F_Language = age.ToString();

            User u = new User();
            u.Id = 1;
            u.Name = "roc";
            u.F_Account = "不知道";

            //sql.Select(m => new { m.F_Account, m.Name, m.F_WeChat });
            //sql.Where(m => m.F_LastModifyTime == DateTime.Now.AddDays(3));
            //sql.Delete().Join<UserLogon>((a, b) => a.F_Id == u.Name, aliasName: "ul").Where(m => m.F_UserId == u.F_Account);
            //sql.Where(m => m.Id == 1);

            //sql.Update(u, m => new { m.F_Account, m.F_RoleId }).Join<UserLogon>((a, b) => a.F_Id == u.Name, aliasName: "ul").Where(m => m.F_UserId == u.F_Account);
            //sql.Where(m => m.Id == 1);

            List<User> areas = new List<User>();
            areas.Add(new User() { F_Id = "1" });
            areas.Add(new User() { F_CreatorTime = DateTime.Now });

            //User[] us = new User[] { };
            //sql.Insert(areas, m => new { m.Id, m.Name, m.F_WeChat });
            //sql.Insert(areas, m => new { m.F_WeChat, m.Name, m.F_SortCode });
            //sql.Insert(new { Id = 1, Name = "Roc" }, increment: true);

            //SqlLam<UserLogon> sqlUl = new SqlLam<UserLogon>("ul");
            //sqlUl.Select(m => new { F_WeChat = m.F_UserId, Name = m.F_Theme, F_SortCode = m.F_Question }).Where(m => m.F_UserId == "aa");

            //sql.InsertWithQuery(m => new { m.F_WeChat, m.Name, m.F_SortCode }, sqlUl);

            string sqlString = sql.GetSql();

            Console.WriteLine(string.Format("SQL: {0}", sqlString));
            var ps = sql.GetParameters();
            foreach (var item in ps)
            {
                Console.WriteLine(string.Format("Key: {0}, Value: {1}", item.Key, item.Value));
            }

            Console.Read();
        }

        static void Main_proc(string[] args)
        {
            SqlEntity sql = new SqlEntity("proc_test", type: CommandType.StoredProcedure);
            sql.AddParameter("a", "chepeng");
            sql.AddParameter("b", "", ParameterDirection.Output);
            sql.AddParameter("@c", "", ParameterDirection.ReturnValue);
            //sql.AddParameters(new List<SqlParameterEntity>() { });
            //sql.Parameters = sql.CreateParameters(dic);
            //sql.Parameters.ElementAt(1).Direction = ParameterDirection.Output;
            //sql.Parameters.ElementAt(1).Size = 100;
            using (var db = new DbHelper(sql))
            {
                string aaa = db.GetList<string>().FirstOrDefault();
                string bbb = sql.Get<string>("b");
                int age = sql.Get<int>("@c");

                Console.WriteLine(aaa);
                Console.WriteLine(bbb);
                Console.WriteLine(age);
            }

            Console.Read();
        }

        static void Main4(string[] args)
        {
            //DynamicParameters dy = new DynamicParameters();
            //dy.Add("@a", "chepeng");
            //dy.Add("@b", "", direction: ParameterDirection.Output);

            //using (var conn = GetConnection())
            //{
            //    string aaa = conn.Query<string>("proc_test", dy, commandType: CommandType.StoredProcedure).FirstOrDefault();
            //    string bbb = dy.Get<string>("@b");

            //    Console.WriteLine(aaa);
            //    Console.WriteLine(bbb);
            //}

            string sqlText = @"SELECT * FROM dbo.Sys_Items AS a LEFT JOIN dbo.Sys_ItemsDetail AS b ON a.F_Id=b.F_ItemId";

            Dictionary<string, Item> dic = new Dictionary<string, Item>();
            using (var conn = GetConnection())
            {
                var list = conn.Query<Item, ItemDetail, Item>(sqlText, (i, d) =>
                {
                    Item item;
                    bool flag = dic.TryGetValue(i.F_Id, out item);
                    if (!flag)
                    {
                        item = i;
                        dic.Add(i.F_Id, i);
                        return item;
                    }
                    return null;
                }, splitOn: "F_Id");
            }

            Console.Read();
        }

        static void Main0(string[] args)
        {
            DbClient db = new DbClient();
            //string sqlText = "SELECT * FROM dbo.Sys_Items AS a LEFT JOIN dbo.Sys_ItemsDetail AS b ON a.F_Id=b.F_ItemId";
            string sqlText = @"SELECT * FROM dbo.Sys_Items
SELECT * FROM dbo.Sys_ItemsDetail WHERE F_ItemId IN(
SELECT F_Id FROM dbo.Sys_Items 
)";

            var reader = db.GetReader(sqlText, null);
            Dictionary<string, Item> dic = new Dictionary<string, Item>();
            //var list = reader.Read<Item, ItemDetail, Item>((i, d) =>
            //{
            //    Item item;
            //    bool flag = dic.TryGetValue(i.F_Id, out item);
            //    if (!flag)
            //    {
            //        item = i;
            //        dic.Add(i.F_Id, i);
            //    }
            //    if (d != null)
            //    {
            //        if (i.F_Id == d.F_ItemId)
            //            item.Details.Add(d);
            //    }
            //    return i;
            //}, "F_Id");
            //var list = reader.MultiRead<string, Item, ItemDetail, Item>(m => m.F_Id, m => m.F_ItemId, (i, details) =>
            //{
            //    if (details != null)
            //        i.Details.AddRange(details);
            //    return i;
            //});

            var list = reader.MultiRead<string, Item, ItemDetail>(m => m.F_Id, m => m.F_ItemId, (i, details) => { i.Details = details; });
            foreach (var item in list)
            {
                int count = item.Details == null ? 0 : item.Details.Count();
                Console.WriteLine(string.Format("Code: {0}, Name: {1}, Count: {2}", item.F_EnCode, item.FullName, count));
            }
            Console.Read();
        }

        static void Main5(string[] args)
        {
            //GlobalConfig.UseDb(ProviderType.Oracle);

            SqlLam<User> sql = new SqlLam<User>("u", ProviderType.SQLite);
            sql.QueryPage(2).OrderBy(m => m.F_Email);

            string sqlString = sql.GetSql();

            Console.WriteLine(string.Format("SQL: {0}", sqlString));
            var ps = sql.GetParameters();
            foreach (var item in ps)
            {
                Console.WriteLine(string.Format("Key: {0}, Value: {1}", item.Key, item.Value));
            }
            //测试字典
            Console.Read();
        }

        public static IDbConnection GetConnection()
        {
            string connectString = ConfigurationManager.ConnectionStrings["connectString"].ConnectionString;
            return new SqlConnection(connectString);
        }

        public static string GetId(string age) { return age; }
        public static int GetId(int age) { return age; }

        private static bool IsNull(string s, string ss)
        {
            return s == ss;
        }

        public static int ExecuteNoQuery(string sql, Dictionary<string, object> dic)
        {
            SqlEntity model = new SqlEntity(sql);
            model.AddParameters(dic);
            model.OnDbError = (s, e) =>
            {
                Console.WriteLine(string.Format("错误信息: {0}", e.Message));
            };
            using (DbHelper helper = new DbHelper(model))
            {
                int count = helper.ExecuteNonQuery();
                return count;
            }
        }

        #region test sqlite3

        public void testsqlite3()
        {
            GlobalConfig.UseDb(ProviderType.SQLite);

            string insertSql = "insert into t_user(name,createtime) values('test',datetime())";
            SqlEntity sql = new SqlEntity(insertSql);
            sql.OnDbError = (s, e) =>
            {
                Console.WriteLine("SQL语句: " + s.Text);
                Console.WriteLine("错误信息: " + e.Message);
            };
            using (var db = new DbHelper(sql))
            {
                int count = db.ExecuteNonQuery();
                Console.WriteLine("受影响的行数: " + count);

                sql.Text = "SELECT * FROM t_user";
                var list = db.GetList<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Console.WriteLine(string.Format("id: [{0}] name: [{1}] time: [{2}]", item["Id"], item["Name"], item["CreateTime"]));
                }
            }
        }

        #endregion

        #region test mysql

        private void testmysql()
        {
            GlobalConfig.UseDb(ProviderType.MySql);

            string insertSql = "insert into t_test(name,createtime) values('test',now())";
            SqlEntity sql = new SqlEntity(insertSql);
            sql.OnDbError = (s, e) =>
            {
                Console.WriteLine("SQL语句: " + s.Text);
                Console.WriteLine("错误信息: " + e.Message);
            };
            using (var db = new DbHelper(sql))
            {
                int count = db.ExecuteNonQuery();
                Console.WriteLine("受影响的行数: " + count);

                sql.Text = "SELECT * FROM t_test";
                var list = db.GetList<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Console.WriteLine(string.Format("id: [{0}] name: [{1}] time: [{2}]", item["Id"], item["Name"], item["CreateTime"]));
                }
            }
        }

        #endregion

        #region test dic

        private void testdic()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            Type type = dic.GetType();

            var gtype = type.GetGenericTypeDefinition();
            var gts = type.GetGenericArguments();

            Console.WriteLine(type.Namespace);
            Console.WriteLine(type.Name);

            foreach (var item in gts)
            {
                Console.WriteLine(item.FullName);
            }

            var obj = Activator.CreateInstance(type);
            var addInfo = type.GetMethod("Add");

            addInfo.Invoke(obj, new object[] { "Key1", "Value1" });
            addInfo.Invoke(obj, new object[] { "Key2", "Value2" });

            if (obj is Dictionary<string, object>)
            {
                var myDic = obj as Dictionary<string, object>;
                foreach (var item in myDic)
                {
                    Console.WriteLine(string.Format("K: {0}, V: {1}", item.Key, item.Value));
                }
            }
            var interfaces = type.GetInterfaces();
            foreach (var item in interfaces)
            {
                Console.WriteLine(item.Name);
            }
            var dicInfo = interfaces.FirstOrDefault(m => m.Name.Contains("IDictionary"));
            Console.WriteLine(dicInfo != null);
            //Console.WriteLine(type.Name);
        }

        #endregion

        #region test query

        private void testQuery()
        {
            SqlEntity sql = new SqlEntity("SELECT * FROM dbo.Sys_User");
            DbHelper helper = new DbHelper(sql);
            var list = helper.GetList<User>();

            int index = 0;
            sql.Text = @"SELECT F_Id, F_Account FROM dbo.Sys_Log
            SELECT F_LogOnCount FROM dbo.Sys_UserLogOn
            SELECT * FROM dbo.Sys_User
            SELECT F_EnCode,F_FullName FROM dbo.Sys_Area  WHERE F_Layers=1 ORDER BY F_Id 
            ";
            var reader = helper.GetReader();
            var accounts2 = reader.Read<string[]>();


            foreach (var item in accounts2)
            {
                Console.WriteLine(string.Join(",", item));
            }

            //sql.Text = "SELECT F_LogOnCount FROM dbo.Sys_UserLogOn";
            //var logOns = helper.GetList<double[]>();
            var logOns = reader.Read<int[]>();

            foreach (var item in logOns)
            {
                Console.WriteLine(string.Join(",", item));
            }

            var users = reader.Read<Dictionary<string, object>>();

            foreach (var item in users)
            {
                string msg = string.Format("第{0}个用户,用户名: [{1}] , 手机号: [{2}]", ++index, item["F_RealName"], item["F_MobilePhone"]);
                Console.WriteLine(msg);
            }

            var areas = reader.Read<Dictionary<string, string>>();

            foreach (var item in areas)
            {
                string msg = string.Format("城市代码[{0}], 城市名称: [{1}]", item["F_EnCode"], item["F_FullName"]);
                Console.WriteLine(msg);
            }
        }

        #endregion
    }
}
