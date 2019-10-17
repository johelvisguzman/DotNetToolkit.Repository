namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void ExecuteQuery()
        {
            var exclude = InMemoryContextProviders()
                .Union(FileStreamContextProviders())
                .Union(AzureStorageContextProviders())
                .ToArray();

            ForAllRepositoryFactories(TestExecuteQuery, exclude);
        }

        [Fact]
        public void ExecuteQueryAsync()
        {
            var exclude = InMemoryContextProviders()
                .Union(FileStreamContextProviders())
                .Union(AzureStorageContextProviders())
                .ToArray();

            ForAllRepositoryFactoriesAsync(TestExecuteQueryAsync, exclude);
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
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);

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
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.Single(customersInDb);

            customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("New Random Name", customerInDb.Name);

            // ** DELETE **
            rowsAffected = repo.ExecuteSqlCommand(@"
DELETE FROM NewCustomers
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.Empty(customersInDb);
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
    Name nvarchar(255)
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
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                });

            Assert.Single(customersInDb);

            var customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("Random Name", customerInDb.Name);

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
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                });

            Assert.Single(customersInDb);

            customerInDb = customersInDb.ElementAt(0);

            Assert.NotNull(customerInDb);
            Assert.Equal(1, customerInDb.Id);
            Assert.Equal("New Random Name", customerInDb.Name);

            // ** DELETE **
            rowsAffected = await repo.ExecuteSqlCommandAsync(@"
DELETE FROM NewCustomers
WHERE Id = @p0",
                parameters);

            Assert.Equal(1, rowsAffected);

            customersInDb = await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                parameters,
                r => new Customer()
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                });

            Assert.Empty(customersInDb);
        }
    }
}
