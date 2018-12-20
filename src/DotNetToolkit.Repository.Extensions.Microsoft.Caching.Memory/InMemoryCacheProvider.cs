namespace DotNetToolkit.Repository.Extensions.Microsoft.Caching.Memory
{
    using Configuration.Caching;
    using global::Microsoft.Extensions.Caching.Memory;
    using InMemory.Caching;
    using System;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider" />.
    /// </summary>
    public class InMemoryCacheProvider : ICacheProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="cache">The underlying caching storage.</param>
        public InMemoryCacheProvider(IMemoryCache cache)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            Cache = new InMemoryCache(cache);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        public InMemoryCacheProvider() : this(new MemoryCache(new MemoryCacheOptions())) { }

        #endregion

        #region Implementation of ICacheProvider

        /// <summary>
        /// Gets or sets the caching expiration time.
        /// </summary>
        public TimeSpan? CacheExpiration { get; set; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public ICache Cache { get; }

        #endregion
    }
}
