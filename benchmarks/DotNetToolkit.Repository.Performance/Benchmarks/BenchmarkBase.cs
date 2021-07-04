namespace DotNetToolkit.Repository.Performance.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

#if NETFULL
    using System.Data.SqlClient;
#endif
    using BenchmarkDotNet.Attributes;
    using Configuration.Options;
    using Data;
#if NETFULL
    using EntityFramework;
#endif
    using EntityFrameworkCore;
    using InMemory;
    using Microsoft.EntityFrameworkCore;
#if NETSTANDARD2_1
    using AzureStorageBlob;  
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
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder.UseEntityFrameworkCore<EfCoreDbContext>(x => x.UseSqlServer(ConnectionString));
                        break;
                    }
#if NETFULL
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<EfDbContext>(_connection);
                        break;
                    }
#endif
#if NETSTANDARD2_1
                case ContextProviderType.AzureStorageBlob:
                    {
                        builder.UseAzureStorageBlob(
                            connectionString: "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;",
                            container: Guid.NewGuid().ToString(),
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
#if NETFULL
                ContextProviderType.EntityFramework,
#endif
                ContextProviderType.InMemory,
                ContextProviderType.EntityFrameworkCore,
#if NETSTANDARD2_1
		        ContextProviderType.AzureStorageBlob,
#endif
            };
        }

        [ParamsSource(nameof(Providers))]
        public ContextProviderType Provider { get; set; }
    }
}
