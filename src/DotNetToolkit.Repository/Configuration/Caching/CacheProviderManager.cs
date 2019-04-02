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
        /// Gets the caching prefix.
        /// </summary>
        public static string CachePrefix = "RepositoriesCachingPrefix";

        /// <summary>
        /// Gets the caching counter prefix.
        /// </summary>
        public static string CacheCounterPrefix = "RepositoriesCachingCounterPrefix";

        /// <summary>
        /// Gets the caching glue.
        /// </summary>
        public static string CachePrefixGlue = "§";

        /// <summary>
        /// Gets the global caching prefix counter.
        /// </summary>
        public static int GlobalCachingPrefixCounter { get { return _counter; } }

        /// <summary>
        /// Increments the global caching prefix counter.
        /// </summary>
        public static void IncrementCounter()
        {
            Interlocked.Increment(ref _counter);
        }

        /// <summary>
        /// Decrements the global caching prefix counter.
        /// </summary>
        public static void DecrementCounter()
        {
            Interlocked.Decrement(ref _counter);
        }
    }
}
