namespace DotNetToolkit.Repository.Integration.Test.Fixtures
{
    using System;

    public class RepositoryTestsFixture : IDisposable
    {
        public RepositoryTestsFixture()
        {
#if NETCORE
            Running.AzureStorageEmulatorManager.Start();
#endif
        }

        public void Dispose()
        {
#if NETCORE
            Running.AzureStorageEmulatorManager.Stop();
#endif
        }
    }
}
