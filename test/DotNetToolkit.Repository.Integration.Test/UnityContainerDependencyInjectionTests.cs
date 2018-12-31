namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using Extensions.Unity;
    using Factories;
    using InMemory;
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
                typeof(UnityContainerDependencyInjectionTests).Assembly);

            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(container.Resolve<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptor>());
            Assert.Equal(3, container.ResolveAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(container.Resolve<IRepository<Customer>>());
            Assert.NotNull(container.Resolve<IRepository<Customer, int>>());
            Assert.NotNull(container.Resolve<IRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<ITestCustomerRepository>());
            Assert.NotNull(container.Resolve<IService<Customer>>());
            Assert.NotNull(container.Resolve<IService<Customer, int>>());
            Assert.NotNull(container.Resolve<IService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(container.Resolve<IService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(container.Resolve<ITestCustomerService>());
            Assert.NotNull(container.Resolve<IRepositoryFactory>());
            Assert.NotNull(container.Resolve<IRepositoryOptions>());
            Assert.NotNull(container.Resolve<IUnitOfWork>());
            Assert.NotNull(container.Resolve<IUnitOfWorkFactory>());
            Assert.NotNull(container.Resolve<RepositoryOptionsBuilder>());
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
                typeof(UnityContainerDependencyInjectionTests).Assembly);

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
                typeof(MicrosoftDependencyInjectionTests).Assembly);

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
                options.UseInterceptor(new TestRepositoryInterceptor("RANDOM P1", false));
                options.UseInterceptor(new TestRepositoryTimeStampInterceptor("RANDOM USER"));
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            },
                typeof(UnityContainerDependencyInjectionTests).Assembly);

            var repo = new Repository<Customer>(container.Resolve<RepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Single(container.ResolveAll<IRepositoryInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(container.Resolve<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptor>());
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
                typeof(UnityContainerDependencyInjectionTests).Assembly);

            var repo = new Repository<Customer>(container.Resolve<RepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Equal(3, container.ResolveAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(container.Resolve<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(container.Resolve<TestRepositoryInterceptor>());
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
