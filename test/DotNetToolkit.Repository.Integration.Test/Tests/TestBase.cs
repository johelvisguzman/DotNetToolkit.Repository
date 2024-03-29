namespace DotNetToolkit.Repository.Integration.Test
{
#if NETCORE
    using AzureStorageBlob;
#endif
#if NETFULL
    using Caching.Memcached;
    using EntityFramework;
#endif
    using Caching.Couchbase;
    using Caching.InMemory;
    using Caching.Redis;
    using Configuration.Logging;
    using EntityFrameworkCore;
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
    using Data;

    public abstract class TestBase
    {
        protected ILoggerProvider TestXUnitLoggerProvider { get; }

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestXUnitLoggerProvider = new TestXUnitLoggerProvider(testOutputHelper);
        }

        protected virtual void BeforeTest(CachingProviderType cachingProvider) { }
        protected virtual void AfterTest(CachingProviderType cachingProvider) { }
        protected virtual void BeforeTest(ContextProviderType contextProvider) { }
        protected virtual void AfterTest(ContextProviderType contextProvider) { }

        protected void ForRepositoryFactoryWithAllCachingProviders(ContextProviderType contextProvider, Action<IRepositoryFactory, CachingProviderType> action)
            => CachingProviders()
                .ToList()
                .ForEach(cachingProvider =>
                {
                    var builder = GetRepositoryOptionsBuilder(contextProvider);

                    BeforeTest(cachingProvider);

                    ApplyCachingProvider(cachingProvider, builder);

                    action(new RepositoryFactory(builder.Options), cachingProvider);

                    AfterTest(cachingProvider);
                });

        protected void ForRepositoryFactoryWithAllCachingProviders(ContextProviderType contextProvider, Action<IRepositoryFactory> action)
            => ForRepositoryFactoryWithAllCachingProviders(contextProvider, (factory, type) => action(factory));

        protected void ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType contextProvider, Func<IRepositoryFactory, CachingProviderType, Task> action)
            => CachingProviders()
                .ToList()
                .ForEach(async cachingProvider =>
                {
                    var builder = GetRepositoryOptionsBuilder(contextProvider);

                    BeforeTest(cachingProvider);

                    ApplyCachingProvider(cachingProvider, builder);

                    await HandleExceptionAsync(() => action(new RepositoryFactory(builder.Options), cachingProvider));

                    AfterTest(cachingProvider);
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

                    BeforeTest(x);

                    action(new RepositoryFactory(BuildOptions(x)), x);

                    AfterTest(x);
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

                    BeforeTest(x);

                    await HandleExceptionAsync(() => action(new RepositoryFactory(BuildOptions(x)), x));

                    AfterTest(x);
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

                    BeforeTest(x);

                    action(new ServiceFactory(new UnitOfWorkFactory(BuildOptions(x))), x);

                    AfterTest(x);
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

                    BeforeTest(x);

                    await HandleExceptionAsync(() => action(new ServiceFactory(new UnitOfWorkFactory(BuildOptions(x))), x));

                    AfterTest(x);
                });

        protected void ForAllServiceFactoriesAsync(Func<IServiceFactory, Task> action, params ContextProviderType[] exclude)
            => ForAllServiceFactoriesAsync((factory, type) => action(factory), exclude);

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory, ContextProviderType> action)
            => ContextProviders()
                .Where(SupportsTransactions)
                .ToList()
                .ForEach(x =>
                {
                    BeforeTest(x);
                    
                    action(new UnitOfWorkFactory(BuildOptions(x)), x);
                    
                    AfterTest(x);
                });

        protected void ForAllUnitOfWorkFactories(Action<IUnitOfWorkFactory> action)
            => ForAllUnitOfWorkFactories((factory, type) => action(factory));

        private static bool SupportsTransactions(ContextProviderType x)
            => SqlServerContextProviders()
                .Contains(x);

        private static async Task HandleExceptionAsync(Func<Task> testCode)
        {
            // Perform test
            var task = Record.ExceptionAsync(testCode);

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
#if NETCORE
                case ContextProviderType.AzureStorageBlob:
                    {
                        builder.UseAzureStorageBlob(
                            connectionString: "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;",
                            createIfNotExists: true);
                        break;
                    } 
#endif
#if NETFULL
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<TestEfDbContext>(Helpers.DbConnectionHelper.CreateConnection());
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
                        builder.UseInMemoryCache();

                        break;
                    }
                case CachingProviderType.Redis:
                    {
                        builder.UseRedis(options =>
                        {
                            options
                                .WithEndPoint("localhost")
                                .WithDefaultDatabase(0);
                        });

                        break;
                    }
#if NETFULL
                case CachingProviderType.Memcached:
                    {
                        builder.UseMemcached(options =>
                        {
                            options.WithEndPoint("127.0.0.1", 11211);
                        });

                        break;
                    }
#endif
                case CachingProviderType.Couchbase:
                    {
                        builder.UseCouchbase(options =>
                        {
                            options
                                .WithEndPoint("http://localhost:8091")
                                .WithBucketName("default")
                                .WithUsername("default")
                                .WithPassword("password");
                        });

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
#if NETCORE
            => new[]
            {
		
                ContextProviderType.AzureStorageBlob, 
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
                // CachingProviderType.Couchbase,
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