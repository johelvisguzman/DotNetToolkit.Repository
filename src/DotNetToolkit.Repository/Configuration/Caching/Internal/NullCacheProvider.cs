namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    using System;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    internal class NullCacheProvider : ICacheProvider<NullCache>
    {
        internal static NullCacheProvider Instance { get; } = new NullCacheProvider();

        private NullCacheProvider()
        {
            Cache = NullCache.Instance;
        }

        public TimeSpan? Expiry { get; set; }

        ICache ICacheProvider.Cache
        {
            get { return Cache; }
        }

        public NullCache Cache { get; }
    }
}
