namespace DotNetToolkit.Repository.Performance
{
    using AdoNet;
    using BenchmarkDotNet.Attributes;
    using Configuration.Options;
    using Data;
    using EntityFramework;
    using EntityFrameworkCore;
    using global::NHibernate.Cfg;
    using global::NHibernate.Dialect;
    using global::NHibernate.Driver;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Tool.hbm2ddl;
    using InMemory;
    using Json;
    using Microsoft.EntityFrameworkCore;
    using NHibernate;
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
                case ContextProviderType.Json:
                    {
                        builder.UseJsonDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
                        break;
                    }
                case ContextProviderType.Xml:
                    {
                        builder.UseJsonDatabase(Path.GetTempPath() + Guid.NewGuid().ToString("N"));
                        break;
                    }
                case ContextProviderType.AdoNet:
                    {
                        builder.UseAdoNet(_connection);
                        break;
                    }
                case ContextProviderType.NHibernate:
                    {
                        builder.UseNHibernate(cfg =>
                        {
                            cfg.DataBaseIntegration(x =>
                            {
                                x.Dialect<MsSql2012Dialect>();
                                x.Driver<SqlClientDriver>();
                                x.ConnectionString = ConnectionString;
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
                        builder.UseEntityFramework<EfDbContext>(_connection);
                        break;
                    }
                case ContextProviderType.EntityFrameworkCore:
                    {
                        builder.UseEntityFrameworkCore<EfCoreDbContext>(x => x.UseSqlServer(ConnectionString));
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
                ContextProviderType.EntityFrameworkCore
            };
        }

        [ParamsSource(nameof(Providers))]
        public ContextProviderType Provider { get; set; }
    }
}
