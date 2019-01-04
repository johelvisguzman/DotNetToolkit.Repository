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
        public static int GlobalCachingPrefixCounter { get { return _counter; } }

        /// <summary>
        /// Increments the caching prefix counter.
        /// </summary>
        public static void IncrementCounter()
        {
            Interlocked.Increment(ref _counter);
        }

        /// <summary>
        /// Decrements the caching prefix counter.
        /// </summary>
        public static void DecrementCounter()
        {
            Interlocked.Decrement(ref _counter);
        }
    }
}
