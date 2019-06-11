namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AdoNet;
    using Caching.Couchbase;
    using Caching.InMemory;
    using Caching.Memcached;
    using Caching.Redis;
    using Configuration.Logging;
    using Configuration.Options;
    using EntityFramework;
    using EntityFrameworkCore;
    using global::NHibernate.Cfg;
    using global::NHibernate.Driver;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Tool.hbm2ddl;
    using InMemory;
    using Json;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using NHibernate;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Transactions;
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

        protected void ForRepositoryFactoryWithAllCachingProviders(ContextProviderType contextProvider, Action<IRepositoryFactory, CachingProviderType> action)
        {
            CachingProviders().ToList().ForEach(cachingProvider =>
            {
                var builder = GetRepositoryOptionsBuilder(contextProvider);

                ApplyCachingProvider(cachingProvider, builder);

                action(new RepositoryFactory(builder.Options), cachingProvider);
            });
        }

        protected void ForRepositoryFactoryWithAllCachingProviders(ContextProviderType contextProvider, Action<IRepositoryFactory> action)
            => ForRepositoryFactoryWithAllCachingProviders(contextProvider, (factory, type) => action(factory));

        protected void ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType contextProvider, Func<IRepositoryFactory, CachingProviderType, Task> action)
        {
            CachingProviders().ToList().ForEach(async cachingProvider =>
            {
                var builder = GetRepositoryOptionsBuilder(contextProvider);

                ApplyCachingProvider(cachingProvider, builder);

                await HandleExceptionAsync(() => action(new RepositoryFactory(builder.Options), cachingProvider));
            });
        }

        protected void ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType contextProvider, Func<IRepositoryFactory, Task> action)
            => ForRepositoryFactoryWithAllCachingProvidersAsync(contextProvider, (factory, type) => action(factory));

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory, ContextProviderType> action, params ContextProviderType[] exclude)
        {
            ContextProviders().ToList().ForEach(x =>
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
            ContextProviders().ToList().ForEach(async x =>
            {
                if (exclude != null && exclude.Contains(x))
                    return;

                await HandleExceptionAsync(() => action(new RepositoryFactory(BuildOptions(x)), x));
            });
        }

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllRepositoryFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllServiceFactories(Action<IServiceFactory, ContextProviderType> action, params ContextProviderType[] exclude)
        {
            ContextProviders().Where(SupportsTransactions).ToList().ForEach(x =>
            {
                if (exclude != null && exclude.Contains(x))
                    return;

                action(new ServiceFactory(new UnitOfWorkFactory(BuildOptions(x))), x);
            });
        }

        protected void ForAllServiceFactories(Action<IServiceFactory> action, params ContextProviderType[] exclude)
            => ForAllServiceFactories((factory, type) => action(factory), exclude);

        protected void ForAllServiceFactoriesAsync(Func<IServiceFactory, ContextProviderType, Task> action, params ContextProviderType[] exclude)
        {
            ContextProviders().Where(SupportsTransactions).ToList().ForEach(async x =>
            {
                if (exclude != null && exclude.Contains(x))
                    return;

                await HandleExceptionAsync(() => action(new ServiceFactory(new UnitOfWorkFactory(BuildOptions(x))), x));
            });
        }

        protected void ForAllServiceFactoriesAsync(Func<IServiceFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllServiceFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory, ContextProviderType> action)
        {
            ContextProviders().Where(SupportsTransactions).ToList().ForEach(x =>
            {
                action(new UnitOfWorkFactory(BuildOptions(x)), x);
            });
        }

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
            => ForAllUnitOfWorkFactories((factory, type) => action(factory));

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, ContextProviderType, Task> action)
        {
            ContextProviders().Where(SupportsTransactions).ToList().ForEach(async x =>
            {
                await HandleExceptionAsync(() => action(new UnitOfWorkFactory(BuildOptions(x)), x));
            });
        }

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
            => ForAllUnitOfWorkFactoriesAsync((factory, type) => action(factory));

        protected void ForAllFileStreamContextProviders(Action<IRepositoryOptions, ContextProviderType> action)
        {
            FileStreamContextProviders().ToList().ForEach(x => action(BuildOptions(x), x));
        }

        protected void ForAllFileStreamContextProviders(Action<IRepositoryOptions> action)
            => ForAllFileStreamContextProviders((options, type) => action(options));

        private static bool SupportsTransactions(ContextProviderType x)
        {
            return SqlServerContextProviders().Contains(x);
        }

        private static async Task HandleExceptionAsync(Func<Task> testCode)
        {
            // Perform test
            var task = Record.ExceptionAsync(() => testCode());

            // Checks to see if we have any un-handled exception
            if (task != null)
            {
                var ex = await task;

                Assert.Null(ex);
            }
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
                        builder.UseAdoNet(TestDbConnectionHelper.CreateConnection(), ensureDatabaseCreated: true);
                        break;
                    }
                case ContextProviderType.NHibernate:
                    {
                        builder.UseNHibernate(cfg =>
                        {
                            var currentFile = TestPathHelper.GetTempFileName();
                            var connectionString = $"Data Source={currentFile};Persist Security Info=False";

                            cfg.DataBaseIntegration(x =>
                            {
                                x.Dialect<TestFixedMsSqlCe40Dialect>();
                                x.Driver<SqlServerCeDriver>();
                                x.ConnectionString = connectionString;
                                x.LogSqlInConsole = true;
                                x.LogFormattedSql = true;
                            });

                            var mapper = new ModelMapper();

                            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());

                            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

                            cfg.AddMapping(mapping);

                            var exporter = new SchemaExport(cfg);

                            exporter.Execute(true, true, false);
                        });

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

        private static void ApplyCachingProvider(CachingProviderType cachingProvider, RepositoryOptionsBuilder builder)
        {
            switch (cachingProvider)
            {
                case CachingProviderType.MicrosoftInMemory:
                    {
                        builder.UseCachingProvider(new InMemoryCacheProvider());
                        break;
                    }
                case CachingProviderType.Redis:
                    {
                        var provider = new RedisCacheProvider(allowAdmin: true, defaultDatabase: 0, expiry: null);

                        provider.Cache.Server.FlushAllDatabases();

                        builder.UseCachingProvider(provider);

                        break;
                    }
                case CachingProviderType.Memcached:
                    {
                        var provider = new MemcachedCacheProvider("127.0.0.1", 11211);

                        provider.Cache.Client.FlushAll();

                        builder.UseCachingProvider(provider);

                        break;
                    }
                case CachingProviderType.Couchbase:
                    {
                        var provider = new CouchbaseCacheProvider("http://localhost:8091", "default", "password");

                        using (var bucket = provider.Cache.Cluster.OpenBucket())
                        {
                            bucket.CreateManager("default", "password").Flush();
                        }

                        builder.UseCachingProvider(provider);

                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(cachingProvider));
            }
        }

        protected IRepositoryOptions BuildOptions(ContextProviderType provider) => GetRepositoryOptionsBuilder(provider).Options;

        protected static IEnumerable<ContextProviderType> InMemoryContextProviders()
        {
            return new[]
            {
                ContextProviderType.InMemory,
                ContextProviderType.EntityFrameworkCore
            };
        }

        protected static IEnumerable<ContextProviderType> FileStreamContextProviders()
        {
            return new[]
            {
                ContextProviderType.Json,
                ContextProviderType.Xml
            };
        }

        protected static IEnumerable<ContextProviderType> SqlServerContextProviders()
        {
            return new[]
            {
                ContextProviderType.AdoNet,
                ContextProviderType.NHibernate,
                ContextProviderType.EntityFramework
            };
        }

        private static IEnumerable<ContextProviderType> ContextProviders()
        {
            var list = new List<ContextProviderType>();

            list.AddRange(SqlServerContextProviders());
            list.AddRange(InMemoryContextProviders());
            list.AddRange(FileStreamContextProviders());

            return list;
        }

        private static IEnumerable<CachingProviderType> CachingProviders()
        {
            return new[]
            {
                CachingProviderType.MicrosoftInMemory,
                CachingProviderType.Redis,
                CachingProviderType.Memcached,
                //TODO: Cannot test when Appveyor is running tests.
                //I am not able to find the pre-built binaries so that I can manually run
                //the server (similar to redis and memcached). I am going to comment out testing couchbase for now
                //CachingProviderType.Couchbase,
            };
        }

        public enum ContextProviderType
        {
            InMemory,
            Json,
            Xml,
            AdoNet,
            NHibernate,
            EntityFramework,
            EntityFrameworkCore,
        }

        public enum CachingProviderType
        {
            MicrosoftInMemory,
            Redis,
            Memcached,
            Couchbase,
        }
    }
}
