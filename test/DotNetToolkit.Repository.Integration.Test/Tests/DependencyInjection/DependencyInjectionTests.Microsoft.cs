﻿namespace DotNetToolkit.Repository.Integration.Test.DependencyInjection
{
    using Configuration.Interceptors;
    using Data;
    using Extensions.Microsoft.DependencyInjection;
    using InMemory;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using System;
    using System.Linq;
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

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
            });

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(provider.GetService<TestLoggerProvider>());
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

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<IService<Customer>>();

            service.Create(new Customer());

            Assert.Equal(1, service.GetCount());
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithContextFromOptions()
        {
            var services = new ServiceCollection();

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

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

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInterceptor(new TestRepositoryInterceptor("RANDOM P1", true));
                options.UseInterceptor(new TestRepositoryTimeStampInterceptor("RANDOM USER"));
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var provider = services.BuildServiceProvider();

            var repoOptions = provider.GetService<IRepositoryOptions>();

            Assert.Equal(3, repoOptions.Interceptors.Count());
            Assert.Equal(3, provider.GetServices<IRepositoryInterceptor>().Count());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());

            var registeredInterceptor1 = provider.GetService<TestRepositoryTimeStampInterceptor>();

            Assert.NotNull(registeredInterceptor1);
            Assert.Equal("UNKNOWN_USER", registeredInterceptor1.User);

            var registeredInterceptor2 = provider.GetService<TestRepositoryInterceptor>();

            Assert.NotNull(registeredInterceptor2);
            Assert.Null(registeredInterceptor2.P1);
            Assert.False(registeredInterceptor2.P2);

            var configueredInterceptor1 = (TestRepositoryTimeStampInterceptor)repoOptions.Interceptors[typeof(TestRepositoryTimeStampInterceptor)].Value;

            Assert.NotNull(configueredInterceptor1);
            Assert.Equal("RANDOM USER", configueredInterceptor1.User);

            var configueredInterceptor2 = (TestRepositoryInterceptor)repoOptions.Interceptors[typeof(TestRepositoryInterceptor)].Value;

            Assert.NotNull(configueredInterceptor2);
            Assert.Equal("RANDOM P1", configueredInterceptor2.P1);
            Assert.True(configueredInterceptor2.P2);
        }

        [Fact]
        public void DependencyInjectionCanConfigureRepositoriesWithScannedInterceptors()
        {
            var services = new ServiceCollection();

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var provider = services.BuildServiceProvider();

            var repoOptions = provider.GetService<IRepositoryOptions>();

            Assert.Equal(3, repoOptions.Interceptors.Count());
            Assert.Equal(3, provider.GetServices<IRepositoryInterceptor>().Count());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(provider.GetService<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptor>());
        }

        [Fact]
        public void DependencyInjectionCanConfigureLogginProviderWithScannedLogger()
        {
            var services = new ServiceCollection();

            services.AddRepositories<MicrosoftDependencyInjectionTests>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString(), ignoreTransactionWarning: true);
                options.UseLoggerProvider(TestXUnitLoggerProvider);
            });

            var provider = services.BuildServiceProvider();
            var repoOptions = provider.GetService<IRepositoryOptions>();

            Assert.Equal(typeof(TestXUnitLoggerProvider), repoOptions.LoggerProvider.GetType());

            Assert.Equal(3, repoOptions.Interceptors.Count());
            Assert.Equal(3, provider.GetServices<IRepositoryInterceptor>().Count());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptorWithDependencyInjectedServices>());
            Assert.NotNull(provider.GetService<TestRepositoryTimeStampInterceptor>());
            Assert.NotNull(provider.GetService<TestRepositoryInterceptor>());
        }
    }
}