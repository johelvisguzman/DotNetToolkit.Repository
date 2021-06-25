namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AzureStorageBlob;
    using AzureStorageTable;
    using Caching.Couchbase;
    using Caching.InMemory;
    using Caching.Memcached;
    using Caching.Redis;
    using Configuration.Logging;
    using Configuration.Options;
    using EntityFramework;
    using EntityFrameworkCore;
    using Helpers;
    using InMemory;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Transactions;
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
            => CachingProviders()
                .ToList()
                .ForEach(cachingProvider =>
                {
                    var builder = GetRepositoryOptionsBuilder(contextProvider);

                    ApplyCachingProvider(cachingProvider, builder);

                    action(new RepositoryFactory(builder.Options), cachingProvider);
                });

        protected void ForRepositoryFactoryWithAllCachingProviders(ContextProviderType contextProvider, Action<IRepositoryFactory> action)
            => ForRepositoryFactoryWithAllCachingProviders(contextProvider, (factory, type) => action(factory));

        protected void ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType contextProvider, Func<IRepositoryFactory, CachingProviderType, Task> action)
            => CachingProviders()
                .ToList()
                .ForEach(async cachingProvider =>
                {
                    var builder = GetRepositoryOptionsBuilder(contextProvider);

                    ApplyCachingProvider(cachingProvider, builder);

                    await HandleExceptionAsync(() => action(
                        new RepositoryFactory(builder.Options),
                        cachingProvider));
                });

        protected void ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType contextProvider, Func<IRepositoryFactory, Task> action)
            => ForRepositoryFactoryWithAllCachingProvidersAsync(contextProvider, (factory, type) => action(factory));

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory, ContextProviderType> action, params ContextProviderType[] exclude)
            => ContextProviders()
                .ToList()
                .ForEach(x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    action(new RepositoryFactory(BuildOptions(x)), x);
                });

        protected void ForAllRepositoryFactories(Action<IRepositoryFactory> action, params ContextProviderType[] exclude)
            => ForAllRepositoryFactories((factory, type) => action(factory), exclude);

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, ContextProviderType, Task> action, params ContextProviderType[] exclude)
            => ContextProviders()
                .ToList()
                .ForEach(async x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    await HandleExceptionAsync(() => action(
                        new RepositoryFactory(BuildOptions(x)), x));
                });

        protected void ForAllRepositoryFactoriesAsync(Func<IRepositoryFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllRepositoryFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllServiceFactories(Action<IServiceFactory, ContextProviderType> action, params ContextProviderType[] exclude)
            => ContextProviders()
                .Where(SupportsTransactions)
                .ToList()
                .ForEach(x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    action(new ServiceFactory(new UnitOfWorkFactory(BuildOptions(x))), x);
                });

        protected void ForAllServiceFactories(Action<IServiceFactory> action, params ContextProviderType[] exclude)
            => ForAllServiceFactories((factory, type) => action(factory), exclude);

        protected void ForAllServiceFactoriesAsync(Func<IServiceFactory, ContextProviderType, Task> action, params ContextProviderType[] exclude)
            => ContextProviders()
                .Where(SupportsTransactions)
                .ToList()
                .ForEach(async x =>
                {
                    if (exclude != null && exclude.Contains(x))
                        return;

                    await HandleExceptionAsync(() => action(
                        new ServiceFactory(
                            new UnitOfWorkFactory(
                                BuildOptions(x))), x));
                });

        protected void ForAllServiceFactoriesAsync(Func<IServiceFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllServiceFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory, ContextProviderType> action)
            => ContextProviders()
                .Where(SupportsTransactions)
                .ToList()
                .ForEach(x => action(new UnitOfWorkFactory(BuildOptions(x)), x));

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
            => ForAllUnitOfWorkFactories((factory, type) => action(factory));

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, ContextProviderType, Task> action)
            => ContextProviders()
                .Where(SupportsTransactions)
                .ToList()
                .ForEach(async x => await HandleExceptionAsync(
                    () => action(
                        new UnitOfWorkFactory(
                            BuildOptions(x)), x)));

        protected void ForAllUnitOfWorkFactoriesAsync(Func<IUnitOfWorkFactory, Task> action)
            => ForAllUnitOfWorkFactoriesAsync((factory, type) => action(factory));

        private static bool SupportsTransactions(ContextProviderType x)
            => SqlServerContextProviders()
                .Contains(x);

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
#if NETFULL
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<TestEfDbContext>(DbConnectionHelper.CreateConnection());
                        break;
                    }
                case ContextProviderType.AzureStorageBlob:
                    {
                        builder.UseAzureStorageBlob(
                            nameOrConnectionString: "AzureStorageBlobConnection",
                            container: Guid.NewGuid().ToString(),
                            createIfNotExists: true);
                        break;
                    }
                case ContextProviderType.AzureStorageTable:
                    {
                        builder.UseAzureStorageTable(
                            nameOrConnectionString: "AzureStorageTableConnection",
                            tableName: "TableName" + Guid.NewGuid().ToString("N").ToUpper(),
                            createIfNotExists: true);
                        break;
                    }
#endif
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
#if NETFULL
                case CachingProviderType.Memcached:
                    {
                        var provider = new MemcachedCacheProvider("127.0.0.1", 11211);

                        provider.Cache.Client.FlushAll();

                        builder.UseCachingProvider(provider);

                        break;
                    }
#endif
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

        protected IRepositoryOptions BuildOptions(ContextProviderType provider)
            => GetRepositoryOptionsBuilder(provider).Options;

        protected static ContextProviderType[] InMemoryContextProviders()
            => new[]
            {
                ContextProviderType.InMemory,
		        ContextProviderType.EntityFrameworkCore,  
            };

        protected static ContextProviderType[] SqlServerContextProviders()
#if NETFULL
            => new[]
            {
                ContextProviderType.EntityFramework,
            };
#else
            => Array.Empty<ContextProviderType>();
#endif

        protected static ContextProviderType[] AzureStorageContextProviders()
#if NETFULL
            => new[]
            {
                ContextProviderType.AzureStorageBlob,
                ContextProviderType.AzureStorageTable,
            };
#else
            => Array.Empty<ContextProviderType>();
#endif

        private static CachingProviderType[] CachingProviders()
            => new[]
            {
                CachingProviderType.MicrosoftInMemory,
                CachingProviderType.Redis,
#if NETFULL
		        CachingProviderType.Memcached,  
#endif
                //TODO: Cannot test when Appveyor is running tests.
                //I am not able to find the pre-built binaries so that I can manually run
                //the server (similar to redis and memcached). I am going to comment out testing couchbase for now
                //CachingProviderType.Couchbase,
            };

        private static ContextProviderType[] ContextProviders()
        {
            var list = new List<ContextProviderType>();

            list.AddRange(SqlServerContextProviders());
            list.AddRange(InMemoryContextProviders());
            list.AddRange(AzureStorageContextProviders());

            return list.ToArray();
        }

        public enum ContextProviderType
        {
            InMemory,
            EntityFramework,
            EntityFrameworkCore,
            AzureStorageBlob,
            AzureStorageTable,
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