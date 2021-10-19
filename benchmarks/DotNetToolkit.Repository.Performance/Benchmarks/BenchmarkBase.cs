namespace DotNetToolkit.Repository.Performance.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using BenchmarkDotNet.Attributes;
    using Configuration.Options;
    using Data;
    using InMemory;
#if NETFULL
    using System.Data.SqlClient;
    using EntityFramework;
#else
    using AzureStorageBlob;  
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
#endif

    public abstract class BenchmarkBase
    {
#if NETFULL
        private SqlConnection _connection;
#endif
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void BaseSetup()
        {
#if NETFULL
            _connection = new SqlConnection(ConnectionString);
            _connection.Open();
#endif
        }

        protected IRepositoryOptions BuildOptions(ContextProviderType provider)
        {
            var builder = new RepositoryOptionsBuilder();

            switch (provider)
            {
                case ContextProviderType.InMemory:
                    {
                        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                        break;
                    }
#if NETFULL
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<EfDbContext>(_connection);
                        break;
                    }
#else
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder.UseEntityFrameworkCore<EfCoreDbContext>(x => x.UseSqlServer(ConnectionString));
                        break;
                    }               
                case ContextProviderType.AzureStorageBlob:
                    {
                        builder.UseAzureStorageBlob(
                            connectionString: "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;",
                            createIfNotExists: true);
                        break;
                    } 
#endif
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }

            return builder.Options;
        }

        public virtual IEnumerable<ContextProviderType> Providers()
        {
            return new[]
            {
                ContextProviderType.InMemory,
#if NETFULL
                ContextProviderType.EntityFramework,
#else
                ContextProviderType.EntityFrameworkCore,
		        ContextProviderType.AzureStorageBlob,
#endif
            };
        }

        [ParamsSource(nameof(Providers))]
        public ContextProviderType Provider { get; set; }
    }
}
