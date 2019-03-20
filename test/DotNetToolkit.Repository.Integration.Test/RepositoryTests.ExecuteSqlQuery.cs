﻿namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Mapper;
    using Data;
    using Factories;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void ExecuteQuery()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactories(TestExecuteQuery, exclude);
        }

        [Fact]
        public void ExecuteQueryWithDefaultMapper()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactories(TestExecuteQueryWithDefaultMapper, exclude);
        }

        [Fact]
        public void ExecuteQueryWithRegisteredMapper()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactories(TestExecuteQueryWithRegisteredMapper, exclude);
        }

        [Fact]
        public void ExecuteQueryAsync()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactoriesAsync(TestExecuteQueryAsync, exclude);
        }

        [Fact]
        public void ExecuteQueryWithDefaultMapperAsync()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactoriesAsync(TestExecuteQueryWithDefaultMapperAsync, exclude);
        }

        [Fact]
        public void ExecuteQueryWithRegisteredMapperAsync()
        {
            var exclude = InMemoryProviders().Union(FileStreamProviders()).ToArray();

            ForAllRepositoryFactoriesAsync(TestExecuteQueryWithRegisteredMapperAsync, exclude);
        }

        private static void TestExecuteQuery(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = repo.ExecuteSqlCommand(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = repo.ExecuteSqlCommand(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            var customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);

            // ** UPDATE **
            rowsAffected = repo.ExecuteSqlCommand(@"
UPDATE NewCustomers 
SET NewCustomers.Name = 'New Random Name' 
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Single(customersInDb);

            customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("New Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);

            // ** DELETE **
            rowsAffected = repo.ExecuteSqlCommand(@"
DELETE FROM NewCustomers
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Empty(customersInDb);
        }

        private static void TestExecuteQueryWithDefaultMapper(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = repo.ExecuteSqlCommand(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = repo.ExecuteSqlCommand(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            var customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters);

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);
        }

        private static void TestExecuteQueryWithRegisteredMapper(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = repo.ExecuteSqlCommand(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = repo.ExecuteSqlCommand(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            MapperProvider.Instance.Register<Customer>(new TestCustomerMapper());

            var customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters);

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);
        }

        private static async Task TestExecuteQueryAsync(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = await repo.ExecuteSqlCommandAsync(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            var customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);

            // ** UPDATE **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
UPDATE NewCustomers 
SET NewCustomers.Name = 'New Random Name' 
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Single(customersInDb);

            customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("New Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);

            // ** DELETE **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
DELETE FROM NewCustomers
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.Empty(customersInDb);
        }

        private static async Task TestExecuteQueryWithDefaultMapperAsync(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = await repo.ExecuteSqlCommandAsync(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            var customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters);

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);
        }

        private static async Task TestExecuteQueryWithRegisteredMapperAsync(IRepositoryFactory repoFactory)
        {
            const int id = 1;

            var parameters = new object[] { id };

            var repo = repoFactory.Create<Customer>();

            // ** CREATE **
            var rowsAffected = await repo.ExecuteSqlCommandAsync(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            Assert.Equal(-1, rowsAffected);

            // ** INSERT **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
INSERT INTO NewCustomers (Id, Name)
VALUES (@p0, 'Random Name')",
                parameters);

            Assert.Equal(1, rowsAffected);

            MapperProvider.Instance.Register<Customer>(new TestCustomerMapper());

            var customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters);

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);
            Assert.Equal(0, customerInDb.AddressId);
        }
    }
}