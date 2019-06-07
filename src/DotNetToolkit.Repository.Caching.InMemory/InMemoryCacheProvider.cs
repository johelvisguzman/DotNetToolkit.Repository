namespace DotNetToolkit.Repository.Caching.InMemory
{
    using Configuration.Caching;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    public class InMemoryCacheProvider : CacheProviderBase<InMemoryCache>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        public InMemoryCacheProvider() : this((TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="expiry">The caching expiration time.</param>
        public InMemoryCacheProvider([CanBeNull] TimeSpan? expiry) : this(new MemoryCache(new MemoryCacheOptions()), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="optionsAction">The configuration options action.</param>
        public InMemoryCacheProvider([NotNull] Action<MemoryCacheOptions> optionsAction) : this(optionsAction, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="optionsAction">The configuration options action.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public InMemoryCacheProvider([NotNull] Action<MemoryCacheOptions> optionsAction, [CanBeNull] TimeSpan? expiry) : this(new MemoryCache(GetConfigurationOptions(optionsAction)), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="cache">The underlying caching storage.</param>
        public InMemoryCacheProvider([NotNull] IMemoryCache cache) : this(cache, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCacheProvider" /> class.
        /// </summary>
        /// <param name="cache">The underlying caching storage.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public InMemoryCacheProvider([NotNull] IMemoryCache cache, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new InMemoryCache(Guard.NotNull(cache, nameof(cache)));
            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static MemoryCacheOptions GetConfigurationOptions(Action<MemoryCacheOptions> optionsAction)
        {
            Guard.NotNull(optionsAction, nameof(optionsAction));

            var options = new MemoryCacheOptions();

            optionsAction(options);

            return options;
        }

        #endregion
    }
}
