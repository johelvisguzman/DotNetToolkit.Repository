namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Interceptors;
    using Data;
    using Factories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class AppConfigurationTests
    {
        [Fact]
        public void CanConfigureRepositoriesWithDefaultContextFactoryFromAppConfig()
        {
            var repoFactory = new RepositoryFactory(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions());

            var repo = repoFactory.Create<Customer>();

            var fi = typeof(RepositoryBase<Customer>).GetField("_contextFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            var contextFactory = (IRepositoryContextFactory)fi.GetValue(repo);

            Assert.True(contextFactory is InMemoryRepositoryContextFactory);
        }

        [Fact]
        public void CanConfigureRepositoriesWithInterceptorsFromAppConfig()
        {
            var repoFactory = new RepositoryFactory(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions());

            var repo = repoFactory.Create<Customer>();

            var fi = typeof(RepositoryBase<Customer>).GetField("_interceptors", BindingFlags.NonPublic | BindingFlags.Instance);

            var interceptors = (IEnumerable<IRepositoryInterceptor>)fi.GetValue(repo);
            var interceptor = (TestRepositoryInterceptor)interceptors.ElementAt(0);

            Assert.Equal(1, interceptors.Count());
            Assert.Equal("random param", interceptor.P1);
            Assert.Equal(true, interceptor.P2);
        }
    }
}
