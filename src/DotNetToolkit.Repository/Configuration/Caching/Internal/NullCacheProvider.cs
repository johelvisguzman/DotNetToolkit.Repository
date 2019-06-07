namespace DotNetToolkit.Repository.Configuration.Caching.Internal
{
    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    internal class NullCacheProvider : CacheProviderBase<NullCache>
    {
        internal static NullCacheProvider Instance { get; } = new NullCacheProvider();

        private NullCacheProvider()
        {
            Cache = NullCache.Instance;
        }
    }
}
