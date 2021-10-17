namespace DotNetToolkit.Repository.Test.Data
{
    using Configuration.Caching;
    using System;

    public class TestCacheProvider : ICacheProvider
    {
        public TimeSpan? Expiry { get; set; }

        public TestCacheProvider(TimeSpan? expiry = null)
        {
            Expiry = expiry;
        }

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            return 0;
        }

        public void Remove(string key)
        {
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null, Action<string> cacheRemovedCallback = null)
        {
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            return false;
        }
    }
}
