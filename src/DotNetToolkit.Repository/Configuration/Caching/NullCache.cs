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

        public void Set(string key, object value, CacheItemPriority priority, TimeSpan? cacheExpiration, Action<string> cacheRemovedCallback = null) { }

        public void Remove(string key) { }

        public bool TryGetValue(string key, out object value)
        {
            value = null;

            return false;
        }
    }
}
