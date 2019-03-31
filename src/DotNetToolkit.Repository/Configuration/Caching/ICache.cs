namespace DotNetToolkit.Repository.Configuration.Caching
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Represents an interface for caching query data within the repositories.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        void Set(string key, object value, CacheItemPriority priority, TimeSpan? expiry, Action<string> cacheRemovedCallback = null);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(string key);

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        bool TryGetValue(string key, out object value);
    }
}