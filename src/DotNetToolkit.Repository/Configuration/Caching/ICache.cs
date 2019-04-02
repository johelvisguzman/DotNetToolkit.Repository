namespace DotNetToolkit.Repository.Configuration.Caching
{
    using Microsoft.Extensions.Caching.Memory;
    using System;

    /// <summary>
    /// Represents an interface for caching query data within the repositories.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(string key);

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Increments the number stored at key by one
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="incrementValue">The increment value.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The value of key after the increment.</returns>
        int Increment(string key, int defaultValue, int incrementValue, CacheItemPriority priority = CacheItemPriority.Normal);
    }
}