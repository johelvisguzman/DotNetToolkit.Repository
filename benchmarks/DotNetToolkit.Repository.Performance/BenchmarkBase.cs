namespace DotNetToolkit.Repository.Performance
{
    using AzureStorageBlob;
    using AzureStorageTable;
    using BenchmarkDotNet.Attributes;
    using Configuration.Options;
    using Data;
    using EntityFramework;
    using EntityFrameworkCore;
    using InMemory;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;

    public abstract class BenchmarkBase
    {
        private SqlConnection _connection;
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void BaseSetup()
        {
            _connection = new SqlConnection(ConnectionString);
            _connection.Open();
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
                case ContextProviderType.EntityFramework:
                    {
                        builder.UseEntityFramework<EfDbContext>(_connection);
                        break;
                    }
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder.UseEntityFrameworkCore<EfCoreDbContext>(x => x.UseSqlServer(ConnectionString));
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
                ContextProviderType.Json,
                ContextProviderType.Xml,
                ContextProviderType.AdoNet,
                ContextProviderType.NHibernate,
                ContextProviderType.EntityFramework,
                ContextProviderType.EntityFrameworkCore,
                ContextProviderType.AzureStorageBlob,
                ContextProviderType.AzureStorageTable,
            };
        }

        [ParamsSource(nameof(Providers))]
        public ContextProviderType Provider { get; set; }
    }
}
