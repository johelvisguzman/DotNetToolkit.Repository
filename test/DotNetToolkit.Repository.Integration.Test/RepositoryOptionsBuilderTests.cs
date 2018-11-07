﻿namespace DotNetToolkit.Repository.Integration.Test
{
    using AdoNet;
    using Configuration.Options;
    using Data;
    using EntityFramework;
    using EntityFrameworkCore;
    using InMemory;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryOptionsBuilderTests : TestBase
    {
        public RepositoryOptionsBuilderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ConfigureAdoNet()
        {
            var currentFile = TestPathHelper.GetTempFileName();

            var provider = "System.Data.SqlServerCe.4.0";
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";

            if (File.Exists(currentFile))
                File.Delete(currentFile);

            new SqlCeEngine(connectionString).CreateDatabase();

            /* TODO: MULTIPLICITY ISSUE WHEN CREATING CUSTOMERS
               I was not able to get by without creating the CustomerAddresses and Customers.
               There seems to be an issue with the SchemaTableConfigurationHelper... Needs to comeback to it */
            var factory = DbProviderFactories.GetFactory(provider);

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerAddresses (
                                            Id int IDENTITY PRIMARY KEY,
                                            Street nvarchar (100),
                                            City nvarchar (100),
                                            State nvarchar (2),
                                            CustomerId int)";

                    command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE Customers (
                                            Id int IDENTITY PRIMARY KEY,
                                            Name nvarchar (100),
                                            AddressId int)";

                    command.ExecuteNonQuery();
                }
            }

            var options = new RepositoryOptionsBuilder()
                .UseAdoNet(provider, connectionString)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void ConfigureInMemoryDatabase()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void ConfigureEntityFrameworkCore()
        {
            var options = new RepositoryOptionsBuilder()
                .UseEntityFrameworkCore<TestEfCoreDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void ConfigureEntityFramework()
        {
            var currentFile = TestPathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            var options = new RepositoryOptionsBuilder()
                .UseEntityFramework<TestEfDbContext>(conn)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(new Customer());

            Assert.Equal(1, repo.Count());
        }
    }
}