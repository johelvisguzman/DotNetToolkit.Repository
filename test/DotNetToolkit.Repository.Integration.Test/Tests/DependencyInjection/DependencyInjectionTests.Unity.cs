namespace DotNetToolkit.Repository.Integration.Test.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Configuration.Options.Internal;
    using Data;
    using Extensions.Unity;
    using InMemory;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Unity;
    using Xunit;
    using Xunit.Abstractions;

    public class UnityContainerDependencyInjectionTests : TestBase
    {
        public UnityContainerDependencyInjectionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesServices()
        {
            var container = new UnityContainer();

            container.RegisterRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(container.Resolve<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptor>());
            Assert.Equal(3, container.ResolveAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(container.Resolve<IRepository<Customer>>());
            Assert.NotNull(container.Resolve<IRepository<Customer, int>>());
            Assert.NotNull(container.Resolve<IRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<IReadOnlyRepository<Customer>>());
            Assert.NotNull(container.Resolve<IReadOnlyRepository<Customer, int>>());
            Assert.NotNull(container.Resolve<IReadOnlyRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IReadOnlyRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<ITestCustomerRepository>());
            Assert.NotNull(container.Resolve<IService<Customer>>());
            Assert.NotNull(container.Resolve<IService<Customer, int>>());
            Assert.NotNull(container.Resolve<IService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<IReadOnlyService<Customer>>());
            Assert.NotNull(container.Resolve<IReadOnlyService<Customer, int>>());
            Assert.NotNull(container.Resolve<IReadOnlyService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IReadOnlyService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<ITestCustomerService>());
            Assert.NotNull(container.Resolve<IRepositoryFactory>());
            Assert.NotNull(container.Resolve<IRepositoryOptions>());
            Assert.NotNull(container.Resolve<IUnitOfWork>());
            Assert.NotNull(container.Resolve<IUnitOfWorkFactory>());
            Assert.NotNull(container.Resolve<IServiceFactory>());
            Assert.NotNull(container.Resolve<IRepositoryDependencyResolver>());
        }

        [Fact]
        public void DependencyInjectionCanConfigureServices()
        {
            var container = new UnityContainer();

            container.RegisterRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var service = container.Resolve<IService<Customer>>();

            service.Create(new Customer());

            Assert.Equal(1, service.GetCount());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithContextFromOptions()
        {
            var container = new UnityContainer();

            container.RegisterRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var repoOptions = container.Resolve<RepositoryOptions>();
            var repo = new Repository<Customer>(repoOptions);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithInterceptorsFromOptions()
        {
            var container = new UnityContainer();

            container.RegisterRepositories(options =>
            {
                options.UseInterceptor(new TestRepositoryInterceptor("RANDOM P1", true));
                options.UseInterceptor(new TestRepositoryTimeStampInterceptor("RANDOM USER"));
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var repo = new Repository<Customer>(container.Resolve<RepositoryOptions>());

            var configuredInterceptors = GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo);

            Assert.Equal(3, configuredInterceptors.Count());
            Assert.Equal(3, container.ResolveAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());

            var registeredInterceptor1 = container.Resolve<TestRepositoryTimeStampInterceptor>();

            Assert.NotNull(registeredInterceptor1);
            Assert.Equal("UNKNOWN_USER", registeredInterceptor1.User);

            var registeredInterceptor2 = container.Resolve<TestRepositoryInterceptor>();

            Assert.NotNull(registeredInterceptor2);
            Assert.Null(registeredInterceptor2.P1);
            Assert.False(registeredInterceptor2.P2);

            var configueredInterceptor1 = (TestRepositoryTimeStampInterceptor)configuredInterceptors[typeof(TestRepositoryTimeStampInterceptor)].Value;

            Assert.NotNull(configueredInterceptor1);
            Assert.Equal("RANDOM USER", configueredInterceptor1.User);

            var configueredInterceptor2 = (TestRepositoryInterceptor)configuredInterceptors[typeof(TestRepositoryInterceptor)].Value;

            Assert.NotNull(configueredInterceptor2);
            Assert.Equal("RANDOM P1", configueredInterceptor2.P1);
            Assert.True(configueredInterceptor2.P2);
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithScannedInterceptors()
        {
            var container = new UnityContainer();

            container.RegisterRepositories(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                new[] { typeof(MicrosoftDependencyInjectionTests).Assembly });

            var repo = new Repository<Customer>(container.Resolve<RepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Equal(3, container.ResolveAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(container.Resolve<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptor>());
        }

        private static IReadOnlyDictionary<Type, Lazy<IRepositoryInterceptor>> GetLazyInterceptorsOptionsFromPrivateField<T>(object obj)
        {
            var options = (IRepositoryOptions)typeof(T)
                .GetField("_options", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            return options.Interceptors;
        }
    }
}
