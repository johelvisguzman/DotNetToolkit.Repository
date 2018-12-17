namespace DotNetToolkit.Repository.Extensions.InMemory.Caching
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
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="cacheExpiration">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set(object key, object value, CacheItemPriority priority, TimeSpan? cacheExpiration, Action<string> cacheRemovedCallback = null)
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

            if (cacheExpiration.HasValue && cacheExpiration.Value != TimeSpan.Zero)
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(cacheExpiration.Value);

            if (cacheExpiration.HasValue && cacheExpiration.Value == TimeSpan.Zero && priority != CacheItemPriority.NeverRemove)
                policy.Priority = CacheItemPriority.NeverRemove;
            else
                policy.Priority = priority;

            _cache.Set(key, value, policy);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(object key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _cache.TryGetValue(key, out value);
        }

        #endregion
    }
}
