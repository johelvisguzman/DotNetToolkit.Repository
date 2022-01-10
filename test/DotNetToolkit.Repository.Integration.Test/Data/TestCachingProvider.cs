namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Caching;
    using System;
    using System.Collections.Concurrent;

    public class TestCachingProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            return (int)_cache.AddOrUpdate(key, defaultValue, (k, oldValue) => (int)oldValue + 1);
        }

        public void Remove(string key)
        {
            _cache.TryRemove(key, out _);
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null, Action<string> cacheRemovedCallback = null)
        {
            _cache.AddOrUpdate(key, value, (k, oldValue) => value);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out object obj))
            {
                value = (T)obj;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
