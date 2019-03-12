namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Logging;
    using Configuration.Options;
    using Extensions.Microsoft.Caching.Memory;
    using Factories;
    using InMemory;
    using Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Unity.Interception.Utilities;
    using Xml;
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
                .Where(SupportsTransactions)
                .ForEach(x =>
                {
                    action(new UnitOfWorkFactory(BuildOptions(x)));
                });
        }

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
        {
            Providers()
                .Where(SupportsTransactions)
                .ForEach(async x =>
                {
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

        protected void ForAllFileStreamContextProviders(Action<IRepositoryOptions> action)
        {
            FileStreamProviders().Select(BuildOptions).ForEach(action);
        }

        private static bool SupportsTransactions(ContextProviderType x)
        {
            return SqlServerProviders().Contains(x);
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
                case ContextProviderType.Json:
                    {
                        builder = new RepositoryOptionsBuilder();
                        builder.UseJsonDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
                        break;
                    }
                case ContextProviderType.Xml:
                    {
                        builder = new RepositoryOptionsBuilder();
                        builder.UseXmlDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
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

        protected IRepositoryOptions BuildOptions(ContextProviderType provider) => GetRepositoryOptionsBuilder(provider).Options;

        protected static IEnumerable<ContextProviderType> InMemoryProviders()
        {
            return new[]
            {
                ContextProviderType.InMemory,
                ContextProviderType.EntityFrameworkCore
            };
        }

        protected static IEnumerable<ContextProviderType> FileStreamProviders()
        {
            return new[]
            {
                ContextProviderType.Json,
                ContextProviderType.Xml
            };
        }

        protected static IEnumerable<ContextProviderType> SqlServerProviders()
        {
            return new[]
            {
                ContextProviderType.AdoNet,
                ContextProviderType.EntityFramework
            };
        }

        private static IEnumerable<ContextProviderType> Providers()
        {
            var list = new List<ContextProviderType>();

            list.AddRange(SqlServerProviders());
            list.AddRange(InMemoryProviders());
            list.AddRange(FileStreamProviders());

            return list;
        }

        public enum ContextProviderType
        {
            InMemory,
            Json,
            Xml,
            AdoNet,
            EntityFramework,
            EntityFrameworkCore,
        }
    }
}
