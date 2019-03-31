namespace DotNetToolkit.Repository.Configuration.Caching
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// An implementation of <see cref="ICache" />.
    /// </summary>
    internal class NullCache : ICache
    {
        internal static NullCache Instance { get; } = new NullCache();

        private NullCache() { }

        public void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null) { }

        public void Remove(string key) { }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);

            return false;
        }
    }
}
