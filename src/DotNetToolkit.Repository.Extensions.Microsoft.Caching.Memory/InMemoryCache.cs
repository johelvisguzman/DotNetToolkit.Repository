namespace DotNetToolkit.Repository.Extensions.Microsoft.Caching.Memory
{
    using Configuration.Caching;
    using global::Microsoft.Extensions.Caching.Memory;
    using System;

    /// <summary>
    /// An implementation of <see cref="ICache" />.
    /// </summary>
    public class InMemoryCache : ICache
    {
        #region Fields

        private readonly IMemoryCache _cache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCache" /> class.
        /// </summary>
        /// <param name="cache">The underlying caching storage.</param>
        public InMemoryCache(IMemoryCache cache)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            _cache = cache;
        }

        #endregion

        #region Implementation of ICache

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var policy = new MemoryCacheEntryOptions();

            if (cacheRemovedCallback != null)
            {
                policy.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (o, val, reason, state) =>
                    {
                        cacheRemovedCallback(reason.ToString());
                    }
                });
            }

            if (expiry.HasValue && expiry.Value != TimeSpan.Zero)
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(expiry.Value);

            if (expiry.HasValue && expiry.Value == TimeSpan.Zero && priority != CacheItemPriority.NeverRemove)
                policy.Priority = CacheItemPriority.NeverRemove;
            else
                policy.Priority = priority;

            _cache.Set<T>(key, value, policy);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _cache.TryGetValue<T>(key, out value);
        }

        #endregion
    }
}
