using Journals.Infrastructure.Interface;
using System.Collections.Generic;

namespace Journals.Infrastructure
{
    /// <summary>
    /// Caches the items in local memory
    /// </summary>
    public class InMemoryCache : ICache
    {
        private static Dictionary<string, dynamic> _memoryCache = new Dictionary<string, dynamic>();

        public void Clear()
        {
            _memoryCache.Clear();
        }

        public T Get<T>(string key)
        {
            return (_memoryCache.ContainsKey(key)) ? (T)_memoryCache[key] : default(T);
        }

        public void Put<T>(string key, T toCache)
        {
            if (_memoryCache.ContainsKey(key))
                _memoryCache[key] = toCache;
            else
                _memoryCache.Add(key, toCache);
        }

        public void Remove(string key)
        {
            if (_memoryCache.ContainsKey(key))
                _memoryCache.Remove(key);
        }
    }
}