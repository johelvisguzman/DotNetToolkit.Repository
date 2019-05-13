namespace DotNetToolkit.Repository.Caching.InMemory
{
    using Configuration.Caching;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using Utility;

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
        public InMemoryCache([NotNull] IMemoryCache cache)
        {
            _cache = Guard.NotNull(cache, nameof(cache));
        }

        #endregion

        #region Private Methods

        private void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

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

        #endregion

        #region Implementation of ICache

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>([NotNull] string key, T value, TimeSpan? expiry, Action<string> cacheRemovedCallback = null)
        {
            Set<T>(key, value, CacheItemPriority.Normal, expiry, cacheRemovedCallback);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove([NotNull] string key)
        {
            _cache.Remove(Guard.NotEmpty(key, nameof(key)));
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>([NotNull] string key, out T value)
        {
            return _cache.TryGetValue<T>(Guard.NotEmpty(key, nameof(key)), out value);
        }

        /// <summary>
        /// Increments the number stored at key by one
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="incrementValue">The increment value.</param>
        /// <returns>The value of key after the increment.</returns>
        public int Increment([NotNull] string key, int defaultValue, int incrementValue)
        {
            Guard.NotEmpty(key, nameof(key));

            if (!TryGetValue<int>(key, out var current))
                current = defaultValue;

            var value = current + incrementValue;

            Set<int>(key, value, CacheItemPriority.NeverRemove, expiry: null);

            return value;
        }

        #endregion

        #region Implementation of ICache

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cache.Dispose();
        }

        #endregion
    }
}
