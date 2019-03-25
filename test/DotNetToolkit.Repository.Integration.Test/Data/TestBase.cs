namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AdoNet;
    using Configuration.Logging;
    using Configuration.Options;
    using EntityFramework;
    using EntityFrameworkCore;
    using Factories;
    using InMemory;
    using Json;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
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

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory, ContextProviderType> action, params ContextProviderType[] exclude)
        {
            Providers()
                .ForEach(x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    action(new RepositoryFactory(BuildOptions(x)), x);
                });
        }

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params ContextProviderType[] exclude)
            => ForAllRepositoryFactories((factory, type) => action(factory), exclude);

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, ContextProviderType, Task> action, params ContextProviderType[] exclude)
        {
            Providers()
                .ForEach(async x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new RepositoryFactory(BuildOptions(x)), x));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllRepositoryFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory, ContextProviderType> action)
        {
            Providers()
                .Where(SupportsTransactions)
                .ForEach(x =>
                {
                    action(new UnitOfWorkFactory(BuildOptions(x)), x);
                });
        }

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action) 
            => ForAllUnitOfWorkFactories((factory, type) => action(factory));

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, ContextProviderType, Task> action)
        {
            Providers()
                .Where(SupportsTransactions)
                .ForEach(async x =>
                {
                    // Perform test
                    var task = Record.ExceptionAsync(() => action(new UnitOfWorkFactory(BuildOptions(x)), x));

                    // Checks to see if we have any un-handled exception
                    if (task != null)
                    {
                        var ex = await task;

                        Assert.Null(ex);
                    }
                });
        }

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
            => ForAllUnitOfWorkFactoriesAsync((factory, type) => action(factory));

        protected void ForAllFileStreamContextProviders(Action<IRepositoryOptions, ContextProviderType> action)
        {
            FileStreamProviders().ForEach(x => action(BuildOptions(x), x));
        }

        protected void ForAllFileStreamContextProviders(Action<IRepositoryOptions> action) 
            => ForAllFileStreamContextProviders((options, type) => action(options));

        private static bool SupportsTransactions(ContextProviderType x)
        {
            return SqlServerProviders().Contains(x);
        }

        protected RepositoryOptionsBuilder GetRepositoryOptionsBuilder(ContextProviderType provider)
        {
            var builder = new RepositoryOptionsBuilder();

            switch (provider)
            {
                case ContextProviderType.InMemory:
                    {
                        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                        break;
                    }
                case ContextProviderType.Json:
                    {
                        builder.UseJsonDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
                        break;
                    }
                case ContextProviderType.Xml:
                    {
                        builder.UseXmlDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
                        break;
                    }
                case ContextProviderType.AdoNet:
                    {
                        builder.UseAdoNet(TestDbConnectionHelper.CreateConnection());
                        break;
                    }
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<TestEfDbContext>(TestDbConnectionHelper.CreateConnection());
                        break;
                    }
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder.UseEntityFrameworkCore<TestEfCoreDbContext>(options =>
                        {
                            options
                                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                        });
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }

            builder.UseLoggerProvider(TestXUnitLoggerProvider);

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
