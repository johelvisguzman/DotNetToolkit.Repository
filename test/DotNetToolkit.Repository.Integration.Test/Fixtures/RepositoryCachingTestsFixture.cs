namespace DotNetToolkit.Repository.Integration.Test.Fixtures
{
    using System;

    public class RepositoryCachingTestsFixture : IDisposable
    {
        public RepositoryCachingTestsFixture()
        {
            Running.CachingServerManager.StartMemcached();
            Running.CachingServerManager.StartRedis();
        }

        public void Dispose()
        {
            Running.CachingServerManager.StopMemcached();
            Running.CachingServerManager.StopRedis();
        }
    }
}
