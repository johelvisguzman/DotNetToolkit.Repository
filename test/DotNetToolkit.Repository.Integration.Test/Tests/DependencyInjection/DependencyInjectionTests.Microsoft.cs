namespace DotNetToolkit.Repository.Integration.Test.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using Extensions.Microsoft.DependencyInjection;
    using InMemory;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class MicrosoftDependencyInjectionTests : TestBase
    {
        public MicrosoftDependencyInjectionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesServices()
        {
            var services = new ServiceCollection();

            services.AddRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(provider.GetService<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptor>());
            Assert.Equal(3, provider.GetServices<IRepositoryInterceptor>().Count());
            Assert.NotNull(provider.GetService<IRepository<Customer>>());
            Assert.NotNull(provider.GetService<IRepository<Customer, int>>());
            Assert.NotNull(provider.GetService<IRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(provider.GetService<IRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(provider.GetService<IReadOnlyRepository<Customer>>());
            Assert.NotNull(provider.GetService<IReadOnlyRepository<Customer, int>>());
            Assert.NotNull(provider.GetService<IReadOnlyRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(provider.GetService<IReadOnlyRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(provider.GetService<ITestCustomerRepository>());
            Assert.NotNull(provider.GetService<IService<Customer>>());
            Assert.NotNull(provider.GetService<IService<Customer, int>>());
            Assert.NotNull(provider.GetService<IService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(provider.GetService<IService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(provider.GetService<IReadOnlyService<Customer>>());
            Assert.NotNull(provider.GetService<IReadOnlyService<Customer, int>>());
            Assert.NotNull(provider.GetService<IReadOnlyService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(provider.GetService<IReadOnlyService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(provider.GetService<ITestCustomerService>());
            Assert.NotNull(provider.GetService<IRepositoryFactory>());
            Assert.NotNull(provider.GetService<IRepositoryOptions>());
            Assert.NotNull(provider.GetService<IUnitOfWork>());
            Assert.NotNull(provider.GetService<IUnitOfWorkFactory>());
            Assert.NotNull(provider.GetService<IServiceFactory>());
            Assert.NotNull(provider.GetService<IRepositoryDependencyResolver>());
        }

        [Fact]
        public void DependencyInjectionCanConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<IService<Customer>>();

            service.Create(new Customer());

            Assert.Equal(1, service.GetCount());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithContextFromOptions()
        {
            var services = new ServiceCollection();

            services.AddRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var provider = services.BuildServiceProvider();

            var repoOptions = provider.GetService<IRepositoryOptions>();
            var repo = new Repository<Customer>(repoOptions);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithInterceptorsFromOptions()
        {
            var services = new ServiceCollection();

            services.AddRepositories(options =>
            {
                options.UseInterceptor(new TestRepositoryInterceptor("RANDOM P1", false));
                options.UseInterceptor(new TestRepositoryTimeStampInterceptor("RANDOM USER"));
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                    new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var provider = services.BuildServiceProvider();

            var repo = new Repository<Customer>(provider.GetService<IRepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Single(provider.GetServices<IRepositoryInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.Null(provider.GetService<TestRepositoryTimeStampInterceptor>());
            Assert.Null(provider.GetService<TestRepositoryInterceptor>());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithScannedInterceptors()
        {
            var services = new ServiceCollection();

            services.AddRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var provider = services.BuildServiceProvider();

            var repo = new Repository<Customer>(provider.GetService<IRepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Equal(3, provider.GetServices<IRepositoryInterceptor>().Count());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(provider.GetService<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptor>());
        }

        private static IEnumerable<Lazy<IRepositoryInterceptor>> GetLazyInterceptorsOptionsFromPrivateField<T>(object obj)
        {
            var options = (IRepositoryOptions)typeof(T)
                .GetField("_options", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            return options.Interceptors.Values;
        }
    }
}