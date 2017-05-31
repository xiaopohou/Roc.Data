using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class Common
    {
        #region sql text

        internal static string[] LeftTokens = new string[] { "[", "`", "\"" };
        internal static string[] RightTokens = new string[] { "]", "`", "\"" };
        internal static string[] ParamPrefixs = new string[] { "@", "?", ":", "$" };


        internal static string QuerySQLFormatString = @"SELECT {0} FROM {1} {2} {3} {4} {5}";
        internal static string InsertSQLFormatString = @"INSERT INTO {0} ({1}) {2}";
        internal static string UpdateSQLFormatString = @"UPDATE {0} SET {1} FROM {2} {3}";
        internal static string DeleteSQLFormatString = @"DELETE {0} FROM {1} {2}";
        internal static string TruncateSQLFormatString = @"TRUNCATE TABLE {0}";

        public static string SqlserverAutoKeySQLString = "SELECT ISNULL(SCOPE_IDENTITY(),0) AS AutoID";
        public static string SqliteAutoKeySQLString = "SELECT last_insert_rowid() AS AutoID;";
        #endregion

        internal readonly static string IDictionaryName = "IDictionary";
        internal readonly static string DynamicCreateName = "ROC.DATA.DYNAMICCREATE";

        internal readonly static string DynamicReaderCache = "ROC.DATA.DynamicReader_{0}_{1}";
        internal readonly static string DbDriversCache = "ROC.DATA.DbDrivers";

        internal readonly static string EntityPropertyListCache = "ROC.DATA.EntityPropertyList_{0}";
        internal readonly static string DbTypeMapListCache = "ROC.DATA.DbTypeMapList";
        internal readonly static string LinqBinary = "System.Data.Linq.Binary";

        internal readonly static int DefaultLength = 4000;

        public static string GetParamPrefixs(ProviderType type)
        {
            string prefix = string.Empty;
            switch (type)
            {
                case ProviderType.SQLServer2005:
                case ProviderType.SQLServer2012:
                    prefix = ParamPrefixs[0];
                    break;
                case ProviderType.Oledb:
                    prefix = ParamPrefixs[0];
                    break;
                case ProviderType.MySql:
                    prefix = ParamPrefixs[1];
                    break;
                case ProviderType.Oracle:
                    prefix = ParamPrefixs[2];
                    break;
                case ProviderType.SQLite:
                    prefix = ParamPrefixs[0];
                    break;
            }
            return prefix;
        }
    }
}
