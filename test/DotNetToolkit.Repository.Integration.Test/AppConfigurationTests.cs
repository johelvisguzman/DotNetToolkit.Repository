namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using Factories;
    using InMemory;
    using Internal;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public class AppConfigurationTests
    {
        [Fact]
        public void CanConfigureRepositoriesWithDefaultContextFactoryFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .Options;

            var repoFactory = new RepositoryFactory(options);
            var repo = repoFactory.Create<Customer>();
            var contextFactory = GetContextFactoryFromPrivateField<RepositoryBase<Customer>>(repo);

            Assert.True(contextFactory is InMemoryRepositoryContextFactory);
        }

        [Fact]
        public void CanConfigureRepositoriesWithInterceptorsFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .Options;

            var repoFactory = new RepositoryFactory(options);
            var repo = repoFactory.Create<Customer>();
            var interceptors = GetLazyInterceptorsOptionsFromPrivateField<RepositoryBase<Customer>>(repo);

            Assert.Single(interceptors);
        }

        private static IEnumerable<Lazy<IRepositoryInterceptor>> GetLazyInterceptorsOptionsFromPrivateField<T>(object obj)
        {
            var options = (RepositoryOptions)typeof(T)
                .GetField("_options", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            return options.Interceptors;
        }

        private static IRepositoryContextFactory GetContextFactoryFromPrivateField<T>(object obj)
        {
            var contextFactory = (IRepositoryContextFactory)typeof(T)
                .GetField("_contextFactory", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            return contextFactory;
        }
    }
}
