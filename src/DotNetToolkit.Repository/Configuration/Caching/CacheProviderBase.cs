namespace DotNetToolkit.Repository.Configuration.Caching
{
    using System;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    public abstract class CacheProviderBase<TCache> : ICacheProvider<TCache> where TCache : ICache
    {
        #region Implementation of ICacheProvider

        /// <summary>
        /// Gets or sets the cache key transformer.
        /// </summary>
        public ICacheKeyTransformer KeyTransformer { get; set; }

        /// <summary>
        /// Gets or sets the caching expiration time.
        /// </summary>
        public TimeSpan? Expiry { get; set; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        ICache ICacheProvider.Cache
        {
            get { return Cache; }
        }

        #endregion

        #region Implementation of ICacheProvider<TCache>

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public TCache Cache { get; protected set; }

        #endregion
    }
}
