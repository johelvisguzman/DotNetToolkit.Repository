namespace DotNetToolkit.Repository.Integration.Test.Fixtures
{
using System;

    public class RepositoryTestsFixture : IDisposable
    {
        public RepositoryTestsFixture()
        {
            Running.AzureStorageEmulatorManager.Start();
        }

        public void Dispose()
        {
            Running.AzureStorageEmulatorManager.Stop();
        }
    }
}
