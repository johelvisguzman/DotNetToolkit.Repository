namespace DotNetToolkit.Repository.Caching.Couchbase
{
    using Configuration.Caching;
    using global::Couchbase;
    using global::Couchbase.Configuration.Client;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICache" />.
    /// </summary>
    public class CouchbaseCache : ICache
    {
        #region Properties

        /// <summary>
        /// Gets the couchbase cluster.
        /// </summary>
        public Cluster Cluster { get; }

        #endregion

        #region Constructors

#if NETFULL
        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCache" /> class.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the couchbase client.</param>
        public CouchbaseCache([NotNull] string sectionName)
        {
            Cluster = new Cluster(Guard.NotEmpty(sectionName, nameof(sectionName)));
        } 
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCache" /> class.
        /// </summary>
        /// <param name="config">The configuration options to use for the couchbase client.</param>
        public CouchbaseCache([NotNull] ClientConfiguration config)
        {
            Cluster = new Cluster(Guard.NotNull(config, nameof(config)));
        }

        #endregion

        #region Implementation of ICache

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>([NotNull] string key, [CanBeNull] T value, [CanBeNull] TimeSpan? expiry, [CanBeNull] Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            using (var bucket = Cluster.OpenBucket())
            {
                if (expiry.HasValue)
                {
                    bucket.Insert(key, value, expiry.Value);
                }
                else
                {
                    bucket.Insert(key, value);
                }
            }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove([NotNull] string key)
        {
            using (var bucket = Cluster.OpenBucket())
            {
                bucket.Remove(Guard.NotEmpty(key, nameof(key)));
            }
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>([NotNull] string key, out T value)
        {
            Guard.NotEmpty(key, nameof(key));

            using (var bucket = Cluster.OpenBucket())
            {
                var operationResult = bucket.Get<T>(key);

                value = operationResult.Value;

                return operationResult.Success;
            }
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

            using (var bucket = Cluster.OpenBucket())
            {
                var operationResult = bucket.Increment(key, Convert.ToUInt64(defaultValue), Convert.ToUInt64(incrementValue));

                return Convert.ToInt32(operationResult.Value);
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Cluster.Dispose();
        }

        #endregion
    }
}
