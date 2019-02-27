namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Caching;
    using Data;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class CachingTests : TestBase
    {
        public CachingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CacheEnabled()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.AdoNet));

            Assert.True(repo.CacheEnabled);
        }

        [Fact]
        public void CacheDisabled()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.AdoNet)
                .UseCachingProvider(NullCacheProvider.Instance)
                .Options;

            var repo = new Repository<Customer>(options);

            Assert.False(repo.CacheEnabled);
        }

        [Fact]
        public void ExecuteQuery()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.AdoNet));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.ExecuteSqlCommand(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.False(repo.CacheUsed);

            repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.ExecuteSqlQuery(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Find()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Find(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            repo.Find(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Find(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void FindWithId()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Find(0);

            Assert.False(repo.CacheUsed);

            repo.Find(0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Find(0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void FindWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Find(options);

            Assert.False(repo.CacheUsed);

            repo.Find(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Find(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void FindAll()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.FindAll(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            repo.FindAll(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.FindAll(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void FindAllWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.FindAll(options);

            Assert.False(repo.CacheUsed);

            repo.FindAll(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.FindAll(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Count()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Count(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            repo.Count(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Count(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void CountWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Count(options);

            Assert.False(repo.CacheUsed);

            repo.Count(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Count(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Exists()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Exists(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            repo.Exists(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Exists(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void ExistsWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Exists(options);

            Assert.False(repo.CacheUsed);

            repo.Exists(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Exists(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void ToDictionary()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.ToDictionary(x => x.Id);

            Assert.False(repo.CacheUsed);

            repo.ToDictionary(x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.ToDictionary(x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void ToDictionaryWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.ToDictionary(options, x => x.Id);

            Assert.False(repo.CacheUsed);

            repo.ToDictionary(options, x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.ToDictionary(options, x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void GroupBy()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.GroupBy(x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            repo.GroupBy(x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.GroupBy(x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void GroupByWithOptions()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.GroupBy(options, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            repo.GroupBy(options, x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.GroupBy(options, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ExecuteQueryAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.AdoNet));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ExecuteSqlCommandAsync(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255),
    AddressId int
)");

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.False(repo.CacheUsed);

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name,
    NewCustomers.AddressId
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    AddressId = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                });

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            await repo.FindAsync(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindWithIdAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAsync(0);

            Assert.False(repo.CacheUsed);

            await repo.FindAsync(0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAsync(0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAsync(options);

            Assert.False(repo.CacheUsed);

            await repo.FindAsync(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAsync(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindAllAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAllAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindAllWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(options);

            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAllAsync(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task CountAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.CountAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            await repo.CountAsync(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.CountAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task CountWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.CountAsync(options);

            Assert.False(repo.CacheUsed);

            await repo.CountAsync(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.CountAsync(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ExistsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(x => x.Id == 0);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ExistsAsync(x => x.Id == 0);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ExistsWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(options);

            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(options);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ExistsAsync(options);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ToDictionaryAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(x => x.Id);

            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ToDictionaryAsync(x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ToDictionaryWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task GroupByAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.GroupByAsync(x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task GroupByWithOptionsAsync()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(options, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(options, x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.GroupByAsync(options, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }
    }
}
