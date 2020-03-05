namespace DotNetToolkit.Repository.Integration.Test.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using Extensions.Ninject;
    using InMemory;
    using Ninject;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class NinjectDependencyInjectionTests : TestBase
    {
        public NinjectDependencyInjectionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesServices()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            kernel.BindRepositories<NinjectDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            Assert.NotNull(kernel.Get<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(kernel.Get<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(kernel.Get<TestRepositoryInterceptor>());
            Assert.Equal(3, kernel.GetAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(kernel.Get<IRepository<Customer>>());
            Assert.NotNull(kernel.Get<IRepository<Customer, int>>());
            Assert.NotNull(kernel.Get<IRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(kernel.Get<IRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(kernel.Get<IReadOnlyRepository<Customer>>());
            Assert.NotNull(kernel.Get<IReadOnlyRepository<Customer, int>>());
            Assert.NotNull(kernel.Get<IReadOnlyRepository<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(kernel.Get<IReadOnlyRepository<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(kernel.Get<ITestCustomerRepository>());
            Assert.NotNull(kernel.Get<IService<Customer>>());
            Assert.NotNull(kernel.Get<IService<Customer, int>>());
            Assert.NotNull(kernel.Get<IService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(kernel.Get<IService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(kernel.Get<IReadOnlyService<Customer>>());
            Assert.NotNull(kernel.Get<IReadOnlyService<Customer, int>>());
            Assert.NotNull(kernel.Get<IReadOnlyService<CustomerWithTwoCompositePrimaryKey, int, string>>());
            Assert.NotNull(kernel.Get<IReadOnlyService<CustomerWithThreeCompositePrimaryKey, int, string, int>>());
            Assert.NotNull(kernel.Get<ITestCustomerService>());
            Assert.NotNull(kernel.Get<IRepositoryFactory>());
            Assert.NotNull(kernel.Get<IRepositoryOptions>());
            Assert.NotNull(kernel.Get<IUnitOfWork>());
            Assert.NotNull(kernel.Get<IUnitOfWorkFactory>());
            Assert.NotNull(kernel.Get<IServiceFactory>());
            Assert.NotNull(kernel.Get<IRepositoryDependencyResolver>());
        }

        [Fact]
        public void DependencyInjectionCanConfigureServices()
        {
            var kernel = new StandardKernel();

            kernel.BindRepositories<NinjectDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var service = kernel.Get<IService<Customer>>();

            service.Create(new Customer());

            Assert.Equal(1, service.GetCount());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithContextFromOptions()
        {
            var kernel = new StandardKernel();

            kernel.BindRepositories<NinjectDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var repoOptions = kernel.Get<IRepositoryOptions>();
            var repo = new Repository<Customer>(repoOptions);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithInterceptorsFromOptions()
        {
            var kernel = new StandardKernel();

            kernel.BindRepositories<NinjectDependencyInjectionTests>(options =>
            {
                options.UseInterceptor(new TestRepositoryInterceptor("RANDOM P1", true));
                options.UseInterceptor(new TestRepositoryTimeStampInterceptor("RANDOM USER"));
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var repo = new Repository<Customer>(kernel.Get<IRepositoryOptions>());

            var configuredInterceptors = GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo);

            Assert.Equal(3, configuredInterceptors.Count());
            Assert.Equal(3, kernel.GetAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(kernel.Get<TestRepositoryInterceptorWithDependencyInjectedServices>());

            var registeredInterceptor1 = kernel.Get<TestRepositoryTimeStampInterceptor>();

            Assert.NotNull(registeredInterceptor1);
            Assert.Equal("UNKNOWN_USER", registeredInterceptor1.User);

            var registeredInterceptor2 = kernel.Get<TestRepositoryInterceptor>();

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
            var kernel = new StandardKernel();

            kernel.BindRepositories<NinjectDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var repo = new Repository<Customer>(kernel.Get<IRepositoryOptions>());

            Assert.Equal(3, GetLazyInterceptorsOptionsFromPrivateField<InternalRepositoryBase<Customer>>(repo).Count());
            Assert.Equal(3, kernel.GetAll<IRepositoryInterceptor>().Count());
            Assert.NotNull(kernel.Get<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(kernel.Get<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(kernel.Get<TestRepositoryInterceptor>());
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
