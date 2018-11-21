namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using Factories;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;
    using Xunit.Abstractions;

    public class AppConfigurationTests : TestBase
    {
        public AppConfigurationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CanConfigureRepositoriesWithDefaultContextFactoryFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repoFactory = new RepositoryFactory(options);
            var repo = repoFactory.Create<Customer>();
        }

        [Fact]
        public void CanConfigureRepositoriesWithInterceptorsFromAppConfig()
        {
            var options = new RepositoryOptionsBuilder()
                .UseConfiguration()
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repoFactory = new RepositoryFactory(options);
            var repo = repoFactory.Create<Customer>();
            var interceptors = GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo);

            Assert.Single(interceptors);
        }

        private static IEnumerable<Lazy<IRepositoryInterceptor>> GetLazyInterceptorsOptionsFromPrivateField<T>(object obj)
        {
            var options = (RepositoryOptions)typeof(T)
                .GetField("_options", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            return options.Interceptors;
        }
    }
}
