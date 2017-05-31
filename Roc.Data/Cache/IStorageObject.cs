using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Cache
{
    public interface IStorageObject<T>
    {
        //public int Minutes = 60;
        //public int Hour = 60 * 60;
        //public int Day = 60 * 60 * 24;

        void Add(string key, T value);
        void Add(string key, T value, int minutes);

        bool ContainsKey(string key);
        T Get(string key);
        IEnumerable<string> GetAllKey();
        void Remove(string key);
        void RemoveAll();
        void RemoveAll(Func<string, bool> removeExpression);
        T this[string key] { get; }
    }
}
