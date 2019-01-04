namespace DotNetToolkit.Repository.Test
{
    using Configuration.Caching;
    using Xunit;

    public class CachingProviderManagerTests
    {
        [Fact]
        public void IncrementCounter()
        {
            var counter = CacheProviderManager.GlobalCachingPrefixCounter;

            CacheProviderManager.IncrementCounter();

            Assert.Equal(counter + 1, CacheProviderManager.GlobalCachingPrefixCounter);
        }

        [Fact]
        public void DecrementCounter()
        {
            var counter = CacheProviderManager.GlobalCachingPrefixCounter;

            CacheProviderManager.DecrementCounter();

            Assert.Equal(counter - 1, CacheProviderManager.GlobalCachingPrefixCounter);
        }
    }
}
