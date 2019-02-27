namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Logging;
    using Configuration.Options;
    using Extensions.Microsoft.Caching.Memory;
    using Factories;
    using InMemory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class TestBase
    {
        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestXUnitLoggerProvider = new TestXUnitLoggerProvider(testOutputHelper);
        }

        protected ILoggerProvider TestXUnitLoggerProvider { get; }

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params ContextProviderType[] contextTypeExceptionList)
        {
            Providers()
                .ToList()
                .ForEach(x =>
                {
                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(x))
                        return;

                    action(new RepositoryFactory(BuildOptions(x)));
                });
        }

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params ContextProviderType[] contextTypeExceptionList)
        {
            Providers()
                .ToList()
                .ForEach(async x =>
                {
                    if (contextTypeExceptionList != null && contextTypeExceptionList.Contains(x))
                        return;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(BuildOptions(x))));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
        {
            Providers()
                .ToList()
                .ForEach(x =>
                {
                    // the in-memory context will not support transactions currently
                    if (x == ContextProviderType.EntityFrameworkCore || x == ContextProviderType.InMemory)
                        return;

                    action(new UnitOfWorkFactory(BuildOptions(x)));
                });
        }

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
        {
            Providers()
                .ToList()
                .ForEach(async x =>
                {
                    // the in-memory context will not support transactions currently
                    if (x == ContextProviderType.EntityFrameworkCore || x == ContextProviderType.InMemory)
                        return;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new UnitOfWorkFactory(BuildOptions(x))));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected RepositoryOptionsBuilder GetRepositoryOptionsBuilder(ContextProviderType provider)
        {
            RepositoryOptionsBuilder builder;

            switch (provider)
            {
                case ContextProviderType.InMemory:
                    {
                        builder = new RepositoryOptionsBuilder();
                        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                        break;
                    }
                case ContextProviderType.AdoNet:
                    {
                        builder = TestAdoNetOptionsBuilderFactory.Create();
                        break;
                    }
                case ContextProviderType.EntityFramework:
                    {
                        builder = TestEfOptionsBuilderFactory.Create();
                        break;
                    }
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder = TestEfCoreOptionsBuilderFactory.Create();
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }

            builder
                .UseCachingProvider(new InMemoryCacheProvider())
                .UseLoggerProvider(TestXUnitLoggerProvider);

            return builder;
        }

        protected IRepositoryOptions BuildOptions(ContextProviderType provider) 
            => GetRepositoryOptionsBuilder(provider)
                .Options;

        private IEnumerable<ContextProviderType> Providers()
        {
            return new[]
            {
                ContextProviderType.InMemory,
                ContextProviderType.AdoNet,
                ContextProviderType.EntityFramework,
                ContextProviderType.EntityFrameworkCore
            };
        }

        public enum ContextProviderType
        {
            InMemory,
            AdoNet,
            EntityFramework,
            EntityFrameworkCore,
        }
    }
}
