using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using Roc.Data.Cache;
using System.Reflection.Emit;

namespace Roc.Data
{
    public class GridReader : IDisposable
    {
        private IDataReader reader;
        private int readCount;

        public GridReader(IDataReader dr)
        {
            this.reader = dr;
            this.readCount = 0;
        }

        public List<T> Read<T>()
        {
            Type type = typeof(T);
            try
            {
                var interfaces = type.GetInterfaces();
                var dicInfo = interfaces.FirstOrDefault(m => m.Name.Contains(Common.IDictionaryName));
                if (dicInfo != null) return ReadDicType<T>(type);
                else if (type.IsValueType || type == typeof(string)) return ReadValueType<T>(type);
                else if (type.IsArray) return ReadArrayType<T>(type);
                else if (type.IsClass) return ReadCustomerType<T>(type);
                else throw new Exception("未知类型,不能解析");
            }
            finally
            {
                NextResult();
            }
        }

        #region 多表一次查询

        public IEnumerable<TFirst> MultiRead<TKey, TFirst, TSecond>(Func<TFirst, TKey> firstKey, Func<TSecond, TKey> secondKey, Action<TFirst, IEnumerable<TSecond>> map)
        {
            var first = this.Read<TFirst>();
            var dic = this.Read<TSecond>().GroupBy(m => secondKey(m)).ToDictionary(m => m.Key, m => m.AsEnumerable());

            foreach (var item in first)
            {
                IEnumerable<TSecond> children;
                if (dic.TryGetValue(firstKey(item), out children))
                {
                    map(item, children);
                }
            }
            return first;
        }

        public IEnumerable<TReturn> MultiRead<TKey, TFirst, TSecond, TReturn>(Func<TFirst, TKey> firstKey, Func<TSecond, TKey> secondKey, Func<TFirst, IEnumerable<TSecond>, TReturn> map)
        {
            var first = this.Read<TFirst>();
            var dic = this.Read<TSecond>().GroupBy(m => secondKey(m)).ToDictionary(m => m.Key, m => m.AsEnumerable());

            List<TReturn> list = new List<TReturn>();
            foreach (var item in first)
            {
                IEnumerable<TSecond> children;
                bool flag = dic.TryGetValue(firstKey(item), out children);
                if (flag)
                {
                    list.Add(map(item, children));
                }
                else
                {
                    list.Add(map(item, null));
                }
            }
            return list;
        }

        #endregion

        #region 多表联合查询
        public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            return this.Read<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(map, splits);
        }

        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            return this.Read<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(map, splits);
        }

        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            return this.Read<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(map, splits);
        }

        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            return this.Read<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(map, splits);
        }

        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            return this.Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(map, splits);
        }

        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TReturn> map, params string[] splits)
        {
            try
            {
                while (reader.Read())
                {
                    var result = this.GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(map, splits);
                    yield return result;
                }
            }
            finally
            {
                NextResult();
            }
        }
        #endregion

        #region private method

        private TReturn GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Delegate map, string[] splits)
        {
            int length = splits == null ? 0 : splits.Length;
            if (length < 1) return default(TReturn);

            List<object> list = new List<object>();

            int splitPoint = 0;
            int currentPos = reader.FieldCount;
            for (int i = length - 1; i >= 0; i--)
            {
                string split = splits[i];
                splitPoint = GetNextSplit(currentPos, split);

                if (splitPoint == -1) return default(TReturn);
                object obj = null;
                switch (i)
                {
                    case 5:
                        obj = this.GetBuilder<TSeventh>(splitPoint, currentPos).Create<TSeventh>(reader);
                        break;
                    case 4:
                        obj = this.GetBuilder<TSixth>(splitPoint, currentPos).Create<TSixth>(reader);
                        break;
                    case 3:
                        obj = this.GetBuilder<TFifth>(splitPoint, currentPos).Create<TFifth>(reader);
                        break;
                    case 2:
                        obj = this.GetBuilder<TFourth>(splitPoint, currentPos).Create<TFourth>(reader);
                        break;
                    case 1:
                        obj = this.GetBuilder<TThird>(splitPoint, currentPos).Create<TThird>(reader);
                        break;
                    case 0:
                        obj = this.GetBuilder<TSecond>(splitPoint, currentPos).Create<TSecond>(reader);
                        break;
                }
                if (obj != null) list.Add(obj);
                currentPos = splitPoint;
            }
            list.Add(this.GetBuilder<TFirst>(0, currentPos).Create<TFirst>(reader));
            list.Reverse();
            if (length == 1)
            {
                var func = (Func<TFirst, TSecond, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1]);
            }
            else if (length == 2)
            {
                var func = (Func<TFirst, TSecond, TThird, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1], (TThird)list[2]);
            }
            else if (length == 3)
            {
                var func = (Func<TFirst, TSecond, TThird, TFourth, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1], (TThird)list[2], (TFourth)list[3]);
            }
            else if (length == 4)
            {
                var func = (Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1], (TThird)list[2], (TFourth)list[3], (TFifth)list[4]);
            }
            else if (length == 5)
            {
                var func = (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1], (TThird)list[2], (TFourth)list[3], (TFifth)list[4], (TSixth)list[4]);
            }
            else if (length == 6)
            {
                var func = (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)map;
                return func((TFirst)list[0], (TSecond)list[1], (TThird)list[2], (TFourth)list[3], (TFifth)list[4], (TSixth)list[4], (TSeventh)list[5]);
            }
            return default(TReturn);
        }

        private int GetNextSplit(int startIdx, string splitOn)
        {
            if (splitOn == "*") return --startIdx;

            for (var i = startIdx - 1; i > 0; --i)
            {
                if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        private void NextResult()
        {
            if (reader.NextResult())
            {
                readCount++;
            }
            else
            {
                this.Dispose();
            }
        }

        private List<T> ReadValueType<T>(Type type)
        {
            List<T> list = new List<T>();
            while (reader.Read())
            {
                var value = reader.GetValue(0);
                if (value != DBNull.Value)
                {
                    list.Add((T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
                }
                else
                {
                    list.Add(default(T));
                }
            }
            return list;
        }
        private List<T> ReadArrayType<T>(Type type)
        {
            var childType = type.GetElementType();
            if (childType == null) throw new ArgumentNullException("childType", "数组类型为null");
            bool flagValue = childType.IsValueType;
            bool flagString = false;
            if (!flagValue) flagString = childType == typeof(string);
            if (flagValue || flagString)
            {
                List<T> list = new List<T>();
                int count = reader.FieldCount;
                while (reader.Read())
                {
                    var obj = Activator.CreateInstance(type, new object[] { count });
                    var setInfo = type.GetMethod("Set");
                    for (int i = 0; i < count; i++)
                    {
                        var v = Convert.ChangeType(GetValue(reader.GetValue(i), flagValue, flagString), childType, CultureInfo.InvariantCulture);
                        setInfo.Invoke(obj, new object[] { i, v });
                    }
                    list.Add((T)obj);
                }
                return list;
            }
            throw new ArgumentNullException("childType", "数组类型为值类型或string类型");
        }
        private List<T> ReadDicType<T>(Type type)
        {
            List<T> list = new List<T>();

            var args = type.GetGenericArguments();
            if (args.FirstOrDefault() == typeof(string))
            {
                int count = reader.FieldCount;
                while (reader.Read())
                {
                    var obj = Activator.CreateInstance<T>();
                    var addInfo = type.GetMethod("Add");
                    for (int i = 0; i < count; i++)
                    {
                        addInfo.Invoke(obj, new object[] { reader.GetName(i), reader.GetValue(i) });
                    }
                    list.Add(obj);
                }
            }
            else
            {
                throw new ArgumentException("字典的Key必须是string类型", "Key");
            }
            return list;
        }
        private List<T> ReadCustomerType<T>(Type type)
        {
            List<T> list = new List<T>();
            var builder = this.GetBuilder<T>(0, reader.FieldCount);

            while (reader.Read())
            {
                list.Add(builder.Build(reader));
            }
            return list;
        }

        private DynamicEntityBuilder<T> GetBuilder<T>(int begin = 0, int end = -1)
        {
            Type type = typeof(T);
            if (type == typeof(DontMap)) return null;
            var cache = CacheHelper<DynamicEntityBuilder<T>>.GetInstance();
            string key = string.Format(Common.DynamicReaderCache, type.Namespace, type.Name);
            var builder = cache.Get(key);
            if (builder == null)
            {
                builder = new DynamicEntityBuilder<T>();
                cache.Add(key, builder);
            }
            builder.BeginField = begin;
            builder.EndField = end;
            return builder;
        }
        private object GetValue(object value, bool f1, bool f2)
        {
            if (value == DBNull.Value)
            {
                if (f1) return 0;
                else if (f2) return string.Empty;
            }
            return value;
        }

        #endregion

        public void Dispose()
        {
            if (reader != null)
            {
                readCount = 0;
                reader.Dispose();
                reader = null;
            }
        }
    }
}
