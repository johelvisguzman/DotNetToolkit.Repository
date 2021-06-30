namespace DotNetToolkit.Repository.Performance
{
#if NETFULL
    using AzureStorageTable;
    using EntityFramework;
    using System.Data.SqlClient;
#endif
#if NETSTANDARD2_1
    using AzureStorageBlob;  
#endif
    using BenchmarkDotNet.Attributes;
    using Configuration.Options;
    using Data;
    using EntityFrameworkCore;
    using InMemory;
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.Configuration;

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
                case ContextProviderType.AzureStorageTable:
                    {
                        builder.UseAzureStorageTable(
                            nameOrConnectionString: "AzureStorageTableConnection",
                            tableName: "TableName" + Guid.NewGuid().ToString("N").ToUpper(),
                            createIfNotExists: true);
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
                ContextProviderType.AzureStorageTable,
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
