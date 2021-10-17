namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    using System;

    internal class NullCacheProvider : ICacheProvider
    {
        internal static NullCacheProvider Instance { get; } = new NullCacheProvider();
        
        public TimeSpan? Expiry { get; set; }

        private NullCacheProvider() { }

        public void Set<T>(string key, T value, TimeSpan? expiry, Action<string> cacheRemovedCallback = null) { }

        public void Remove(string key) { }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);

            return false;
        }

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            return 1;
        }
    }
}
