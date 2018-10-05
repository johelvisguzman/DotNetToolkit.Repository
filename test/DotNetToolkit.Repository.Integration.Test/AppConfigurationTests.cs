namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration;
    using Configuration.Interceptors;
    using Data;
    using Extensions.Microsoft.DependencyInjection;
    using Factories;
    using Microsoft.Extensions.DependencyInjection;
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

            TestContextFactoryFromPrivateField<RepositoryBase<Customer>, InMemoryRepositoryContextFactory>(repo);
        }

        [Fact]
        public void CanConfigureRepositoriesWithInterceptorsFromAppConfig()
        {
            var repoFactory = new RepositoryFactory(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions());

            var repo = repoFactory.Create<Customer>();

            TestInterceptorsFromPrivateField<RepositoryBase<Customer>>(repo);
        }

        [Fact]
        public void CanConfigureRepositoriesWithDependencyInjectionAndConfiguration()
        {
            var services = new ServiceCollection();
            var config = TestConfigurationHelper.GetConfiguration();

            services.AddRepositories(config);

            var provider = services.BuildServiceProvider();

            var repo = provider.GetService<IRepository<Customer>>();

            TestContextFactoryFromPrivateField<RepositoryBase<Customer>, InMemoryRepositoryContextFactory>(repo);
            TestInterceptorsFromPrivateField<RepositoryBase<Customer>>(repo);

            repo = provider.GetService<ITestCustomerRepository>();

            TestContextFactoryFromPrivateField<RepositoryBase<Customer>, InMemoryRepositoryContextFactory>(repo);
            TestInterceptorsFromPrivateField<RepositoryBase<Customer>>(repo);

            Assert.NotNull(provider.GetService<IRepositoryContextFactory>());
            Assert.NotNull(provider.GetService<IRepositoryConfigurationOptions>());
            Assert.NotNull(provider.GetService<IRepositoryInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptor>());
            Assert.NotNull(provider.GetService<IRepositoryFactory>());
        }

        private static void TestInterceptorsFromPrivateField<T>(object obj)
        {
            var interceptors = (IEnumerable<IRepositoryInterceptor>)typeof(T)
                .GetField("_interceptors", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            var interceptor = (TestRepositoryInterceptor)interceptors.ElementAt(0);

            Assert.Equal("random param", interceptor.P1);
            Assert.Equal(true, interceptor.P2);
        }

        private static void TestContextFactoryFromPrivateField<T, TCompareToType>(object obj)
        {
            var contextFactory = (IRepositoryContextFactory)typeof(T)
                .GetField("_contextFactory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            Assert.True(contextFactory is TCompareToType);
        }
    }
}
