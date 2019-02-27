namespace DotNetToolkit.Repository.Performance
{
    using AdoNet;
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
            switch (provider)
            {
                case ContextProviderType.InMemory:
                    {
                        return new RepositoryOptionsBuilder()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;
                    }
                case ContextProviderType.AdoNet:
                    {
                        return new RepositoryOptionsBuilder()
                            .UseAdoNet(_connection)
                            .Options;
                    }
                case ContextProviderType.EntityFramework:
                    {
                        return new RepositoryOptionsBuilder()
                            .UseEntityFramework<EfDbContext>(_connection)
                            .Options;
                    }
                case ContextProviderType.EntityFrameworkCore:
                    {
                        return new RepositoryOptionsBuilder()
                            .UseEntityFrameworkCore<EfCoreDbContext>(x => x.UseSqlServer(ConnectionString))
                            .Options;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }

        public virtual IEnumerable<ContextProviderType> Providers()
        {
            return new[]
            {
                ContextProviderType.InMemory,
                ContextProviderType.AdoNet,
                ContextProviderType.EntityFramework,
                ContextProviderType.EntityFrameworkCore
            };
        }

        [ParamsSource(nameof(Providers))]
        public ContextProviderType Provider { get; set; }
    }
}
