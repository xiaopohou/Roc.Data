using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Cache
{
    public class CacheHelper<T> : IStorageObject<T>
    {
        #region 全局变量
        private static CacheHelper<T> _instance = null;
        private static readonly object _instanceLock = new object();
        private readonly ConcurrentDictionary<string, T> InstanceCache;
        #endregion

        #region 构造函数

        private CacheHelper()
        {
            InstanceCache = new ConcurrentDictionary<string, T>();
        }
        #endregion

        #region  属性
        /// <summary>         
        ///根据key获取value     
        /// </summary>         
        /// <value></value>      
        public T this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }
        #endregion

        #region 公共函数

        /// <summary>         
        /// 验证key是否存在       
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns> /// 	存在<c>true</c> 不存在<c>false</c></returns>         
        public bool ContainsKey(string key)
        {
            return this.InstanceCache.ContainsKey(key);
        }

        /// <summary>         
        /// 根据key获取value  
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns></returns>         
        public T Get(string key)
        {
            if (this.ContainsKey(key))
                return this.InstanceCache[key];
            else
                return default(T);
        }

        /// <summary>         
        /// 获取实例 （单例模式）       
        /// </summary>         
        /// <returns></returns>         
        public static CacheHelper<T> GetInstance()
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new CacheHelper<T>();
            }
            return _instance;
        }

        /// <summary>         
        /// 插入缓存(默认20分钟)        
        /// </summary>         
        /// <param name="key"> key</param>         
        /// <param name="value">value</param>          
        public void Add(string key, T value)
        {
            this.InstanceCache.GetOrAdd(key, value);
        }

        /// <summary>         
        /// 插入缓存        
        /// </summary>         
        /// <param name="key"> key</param>         
        /// <param name="value">value</param>         
        /// <param name="cacheDurationInSeconds">分钟</param>         
        public void Add(string key, T value, int minutes)
        {
            Add(key, value);
        }

        /// <summary>         
        /// 删除缓存         
        /// </summary>         
        /// <param name="key">key</param>         
        public void Remove(string key)
        {
            T val;
            this.InstanceCache.TryRemove(key, out val);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAll()
        {
            this.InstanceCache.Clear();
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        /// <param name="removeExpression">表达式条件</param>
        public void RemoveAll(Func<string, bool> removeExpression)
        {
            var allKeyList = GetAllKey();
            var delKeyList = allKeyList.Where(removeExpression).ToList();
            foreach (var key in delKeyList)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 获取所有缓存key
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllKey()
        {
            return this.InstanceCache.Keys;
        }
        #endregion
    }
}
