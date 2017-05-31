using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Roc.Data.Cache;
using System.IO;

namespace Roc.Data
{
    public class GlobalConfig
    {
        private static List<DbDriver> drivers;
        private static List<SqlTableEntity> tables;

        static GlobalConfig()
        {
            Init();

            AppDomainHelper.SetPrivateBinPath("dll");
        }

        internal static List<SqlTableEntity> Tables { get { return tables; } }

        public static void AddTable(SqlTableEntity table)
        {
            var t = tables.Find(m => m.Type == table.Type && m.Name.Equals(table.Name, StringComparison.CurrentCultureIgnoreCase));
            if (t != null) tables.Remove(t);
            tables.Add(table);
            tables = tables.OrderBy(m => m.Type).ToList();
        }

        public static SqlTableEntity GetTable(string name)
        {
            if (tables != null && tables.Count > 0)
            {
                return tables.FirstOrDefault(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
            return null;
        }

        public static void UseDb(ProviderType type, string name = "")
        {
            DbDriver driver = GetDriver(type, name);
            if (driver != null)
            {
                var assembly = Assembly.LoadFrom(driver.Path);
                driver.AssemblyName = assembly.FullName;
                if (!ExistAssembly(assembly.FullName))
                {
                    AppDomain.CurrentDomain.Load(assembly.FullName);
                }
            }
        }

        public static DbDriver GetDriver(ProviderType type = ProviderType.None, string pname = "")
        {
            return drivers.FirstOrDefault(m =>
            {
                bool flag = true;
                if (type != ProviderType.None) flag = flag && m.Type == type;
                if (!string.IsNullOrEmpty(pname)) flag = flag && m.ProviderName == pname;
                return flag;
            });
        }

        private static bool ExistAssembly(string name)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(m => m.FullName == name);
            return assembly != null;
        }

        private static void Init()
        {
            string key = Common.DbDriversCache;
            var instance = CacheHelper<List<DbDriver>>.GetInstance();
            drivers = instance.Get(key);
            if (drivers == null)
            {
                drivers = new List<DbDriver>();
                drivers.Add(new DbDriver(ProviderType.Oracle, "Oracle.ManagedDataAccess.dll", "Oracle.ManagedDataAccess.Client"));
                drivers.Add(new DbDriver(ProviderType.MySql, "MySql.Data.dll", "MySql.Data.MySqlClient"));
                drivers.Add(new DbDriver(ProviderType.SQLite, "System.Data.SQLite.dll", "System.Data.SQLite"));

                instance.Add(key, drivers);
            }

            tables = new List<SqlTableEntity>();
        }

        public static partial class AppDomainHelper
        {
            public static string AssemblyExtension { get; set; }
            static AppDomainHelper() { AssemblyExtension = "dll"; }

            public static void SetPrivateBinPath(params string[] dirNames)
            {
                var appDomain = AppDomain.CurrentDomain;
                appDomain.AssemblyResolve += (_, resolveEventArgs) =>
                {
                    var searchPattern = "*." + AssemblyExtension;
                    for (var i = 0; i < dirNames.Length; ++i)
                    {
                        var dir = appDomain.BaseDirectory + dirNames[i];
                        if (!Directory.Exists(dir)) continue;
                        var files = Directory.GetFiles(dir, searchPattern);
                        for (var j = 0; j < files.Length; ++j)
                        {
                            var fp = files[j];
                            if (AssemblyName.GetAssemblyName(fp).FullName == resolveEventArgs.Name)
                                return appDomain.Load(File.ReadAllBytes(fp));
                        }
                    }
                    return null;
                };
            }
        }
    }
}
