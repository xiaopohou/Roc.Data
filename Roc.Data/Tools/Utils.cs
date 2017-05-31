using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Roc.Data.Cache;
using System.Collections;
using System.Data;

namespace Roc.Data
{
    public class Utils
    {
        public static string GetUniqueIdentifier(int length)
        {
            int maxSize = length;
            char[] chars = new char[62];
            string a;
            a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            // Unique identifiers cannot begin with 0-9
            if (result[0] >= '0' && result[0] <= '9')
            {
                return GetUniqueIdentifier(length);
            }
            return result.ToString();
        }

        public static SqlTableEntity GetTable(Type type)
        {
            if (type == null) return null;
            SqlTableEntity table = GlobalConfig.GetTable(type.Name);
            if (table == null)
            {
                table = new SqlTableEntity(type);
                table.Columns = new List<SqlColumnEntity>();

                var attr = type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    table.Type = 2;
                    table.TableName = attr.Name;
                }
                GlobalConfig.AddTable(table);
            }
            return table;
        }

        public static SqlColumnEntity GetColumn(SqlTableEntity table, Type type, string columnName)
        {
            SqlColumnEntity column = null;
            if (table != null)
            {
                column = table.GetColumn(columnName);
            }
            if (column == null)
            {
                var p = type.GetProperty(columnName);
                if (p == null) return column;

                var attrs = p.GetCustomAttributes(true);
                if (attrs == null || attrs.Length < 1) return column;

                column = new SqlColumnEntity(columnName);
                int columnType = 1;
                var cattr = attrs.OfType<ColumnAttribute>().FirstOrDefault();
                if (cattr != null)
                {
                    column.FieldName = cattr.Name;
                    columnType = 2;
                }
                var iattr = attrs.OfType<IgnoreAttribute>().FirstOrDefault();
                if (iattr != null)
                {
                    column.Ignore = true;
                    columnType = 2;
                }
                var aattr = attrs.OfType<ActionAttribute>().FirstOrDefault();
                if (aattr != null)
                {
                    column.ActionType = aattr.ActionType;
                    columnType = 2;
                }
                var kattr = attrs.OfType<KeyAttribute>().FirstOrDefault();
                if (kattr != null)
                {
                    column.Key = true;
                    column.Increment = kattr.Increment;
                    columnType = 2;
                }
                column.Type = columnType;
                table.AddColumn(column);
            }
            return column;
        }

        public static bool IsIncrementColumn(string tableName, string columnName)
        {
            var table = GlobalConfig.GetTable(tableName);
            if (table != null)
            {
                var column = table.GetColumn(columnName);
                if (column != null) return column.Increment;
            }
            return false;
        }
        /// <summary>
        /// 获得实体属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreKey">是否忽略 KEY主键</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type type, bool ignoreKey = true, bool ignore = false)
        {
            string key = string.Format(Common.EntityPropertyListCache, type.FullName);
            var instance = CacheHelper<IEnumerable<PropertyInfo>>.GetInstance();
            IEnumerable<PropertyInfo> ps = instance.Get(key);
            if (ps != null && ps.Count() > 0) return ps;

            var props = type.GetProperties();
            if (props == null || props.Length < 1) return null;
            SqlTableEntity table = GetTable(type);
            ps = props.Where(p =>
           {
               SqlColumnEntity column = GetColumn(table, type, p.Name);
               if (column != null)
               {
                   if (ignore && column.Ignore) return false;
                   if ((ignoreKey && column.Key) || column.Increment) return false;
                   if (column.ActionType == ActionType.Undo) return false;
               }
               //var attrs = p.GetCustomAttributes(true);
               //if (ignore)//查询 不管
               //{
               //    var ignoreAttr = attrs.OfType<IgnoreAttribute>().FirstOrDefault() as IgnoreAttribute;
               //    if (ignoreAttr != null) return false;
               //}
               //if (ignoreKey)
               //{
               //    var keyAttr = attrs.OfType<KeyAttribute>().FirstOrDefault() as KeyAttribute;
               //    if (keyAttr != null) return false;
               //}
               ////操作 可读，不能操作 忽略
               //var actionAttr = attrs.OfType<ActionAttribute>().FirstOrDefault() as ActionAttribute;
               //if (actionAttr != null)
               //{
               //    if (actionAttr.ActionType == ActionType.Undo) return false;
               //}
               return IsSimpleType(p.PropertyType);
           });
            instance.Add(key, ps);
            return ps;
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfos<T>(bool ignoreKey, bool ignore)
        {
            return GetPropertyInfos(typeof(T), ignoreKey, ignore);
        }

        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            if (type == null) return null;
            return type.GetProperties();
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            var p = type.GetProperty(name, flag);
            if (p != null) return p;
            return type.GetProperties().FirstOrDefault(m =>
            {
                var column = m.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault(c => c.Name == name);
                return column != null;
            });
        }

        public static object GetPropertyValue(string name, object obj)
        {
            if (obj == null) return null;
            Type type = obj.GetType();
            var p = type.GetProperty(name);
            if (p != null) return p.GetValue(obj, null);
            return DBNull.Value;
        }

        public static List<object> GetObjectList(object value)
        {
            if (value == null) return null;
            Type type = value.GetType();
            if (type.IsClass && type.IsSubclassOf(typeof(IEnumerable)))
            {
                List<object> list = new List<object>();
                IEnumerable ie = (IEnumerable)value;
                var es = ie.GetEnumerator();
                while (es.MoveNext())
                {
                    list.Add(es.Current);
                }
                return list;
            }
            return null;
        }

        public static object GetDefaultValue(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null) return null;
            if (IsSimpleType(type))
            {
                if (type == typeof(string))
                    return null;
                return 0;
            }
            return null;
        }
        /// <summary>
        /// 判断实体属性是否是简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;
            var simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };
            return simpleTypes.Contains(type) || type.IsEnum;
        }

        public static DbType LookupDbType(Type type)
        {
            DbType dbType;
            var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullUnderlyingType != null) type = nullUnderlyingType;
            var typeMap = GetDbTypeMap();
            if (type.IsEnum && !typeMap.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            if (typeMap.TryGetValue(type, out dbType))
            {
                return dbType;
            }
            if (type.FullName == Common.LinqBinary)
            {
                return DbType.Binary;
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return (DbType)(-1);
            }
            //switch (type.FullName)
            //{
            //    case "Microsoft.SqlServer.Types.SqlGeography":
            //        return DbType.Object;
            //    case "Microsoft.SqlServer.Types.SqlGeometry":
            //        return DbType.Object;
            //    case "Microsoft.SqlServer.Types.SqlHierarchyId":
            //        return DbType.Object;
            //}
            return DbType.Object;
        }

        public static Dictionary<Type, DbType> GetDbTypeMap()
        {
            string key = Common.DbTypeMapListCache;
            var instance = CacheHelper<Dictionary<Type, DbType>>.GetInstance();
            Dictionary<Type, DbType> typeMap = instance.Get(key);
            if (typeMap != null && typeMap.Count > 0) return typeMap;

            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan)] = DbType.Time;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan?)] = DbType.Time;
            typeMap[typeof(object)] = DbType.Object;
            instance.Add(key, typeMap);
            return typeMap;
        }
    }
}
