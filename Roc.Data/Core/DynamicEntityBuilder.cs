using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Roc.Data
{
    /// <summary>
    /// 动态填写实体类的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DynamicEntityBuilder<T>
    {
        private int begin;
        private int end;

        public DynamicEntityBuilder()
        {
            begin = 0;
            end = -1;
        }

        public DynamicEntityBuilder(int begin, int end)
        {
            this.begin = begin;
            this.end = end;
        }

        public int BeginField { get { return begin; } set { begin = value; } }

        public int EndField { get { return end; } set { end = value; } }

        public T Build(IDataRecord dr)
        {
            return this.Create<T>(dr);
        }

        public TReturn Create<TReturn>(IDataRecord dr)
        {
            if (dr == null) return default(TReturn);
            Type type = typeof(TReturn);

            var obj = DynamicMethodCompiler.CreateInstantiateObjectHandler(type)();
            for (int i = begin; i < end; i++)
            {
                PropertyInfo p = Utils.GetProperty(type, dr.GetName(i));
                if (p != null)
                {
                    var setHandler = DynamicMethodCompiler.CreateSetHandler(type, p);
                    Type fieldType = dr.GetFieldType(i);
                    object value = dr.GetValue(i);
                    if (value == DBNull.Value) continue;
                    if (fieldType != p.PropertyType)
                    {
                        value = Convert.ChangeType(value, p.PropertyType, CultureInfo.InvariantCulture);
                    }
                    setHandler(obj, value);
                }
            }
            return (TReturn)obj;
        }
    }
}
