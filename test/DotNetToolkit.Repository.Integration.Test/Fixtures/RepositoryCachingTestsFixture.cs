namespace DotNetToolkit.Repository.Integration.Test.Fixtures
{
    using System;

    public class RepositoryCachingTestsFixture : IDisposable
    {
        public RepositoryCachingTestsFixture()
        {
#if NETFULL
            Running.CachingServerManager.StartMemcached();
#endif
            Running.CachingServerManager.StartRedis();
        }

        public void Dispose()
        {
#if NETFULL
            Running.CachingServerManager.StopMemcached();
#endif
            Running.CachingServerManager.StopRedis();
        }
    }
}
