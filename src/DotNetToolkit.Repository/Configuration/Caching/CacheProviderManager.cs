namespace DotNetToolkit.Repository.Configuration.Caching
{
    using System.Threading;

    /// <summary>
    /// Represents a caching provider manager.
    /// </summary>
    public sealed class CacheProviderManager
    {
        private static int _counter = 1;

        /// <summary>
        /// Gets the caching prefix
        /// </summary>
        public static string CachePrefix = "§";

        /// <summary>
        /// Gets the global caching prefix counter.
        /// </summary>
        internal static int GlobalCachingPrefixCounter { get { return _counter; } }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void IncrementCounter()
        {
            Interlocked.Increment(ref _counter);
        }
    }
}
