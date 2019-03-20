namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Caching;
    using Data;
    using Extensions.Microsoft.Caching.Memory;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryCachingTests : TestBase
    {
        public RepositoryCachingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CacheEnabled()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            Assert.True(repo.CacheEnabled);
        }

        [Fact]
        public void CacheDisabled()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .Options;

            var repo = new Repository<Customer>(options);

            Assert.False(repo.CacheEnabled);
            Assert.True(repo.CacheProvider is NullCacheProvider);

            options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(NullCacheProvider.Instance)
                .Options;

            repo = new Repository<Customer>(options);

            Assert.False(repo.CacheEnabled);
        }

        [Fact]
        public void ExecuteQuery()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.AdoNet)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
                    Name = r.GetString(1)
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
                    Name = r.GetString(1)
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
                    Name = r.GetString(1)
                });

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Find()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Find(queryOptions);

            Assert.False(repo.CacheUsed);

            repo.Find(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Find(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void FindAll()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.FindAll(queryOptions);

            Assert.False(repo.CacheUsed);

            repo.FindAll(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.FindAll(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Count()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Count(queryOptions);

            Assert.False(repo.CacheUsed);

            repo.Count(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Count(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void Exists()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.Exists(queryOptions);

            Assert.False(repo.CacheUsed);

            repo.Exists(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.Exists(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void ToDictionary()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.ToDictionary(queryOptions, x => x.Id);

            Assert.False(repo.CacheUsed);

            repo.ToDictionary(queryOptions, x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.ToDictionary(queryOptions, x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public void GroupBy()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.GroupBy(queryOptions, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            repo.GroupBy(queryOptions, x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            repo.GroupBy(queryOptions, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ExecuteQueryAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.AdoNet)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ExecuteSqlCommandAsync(@"
CREATE TABLE NewCustomers (
    Id int,
    Name nvarchar(255)
)");

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.False(repo.CacheUsed);

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ExecuteSqlQueryAsync(@"
SELECT
    NewCustomers.Id,
    NewCustomers.Name
FROM NewCustomers
WHERE NewCustomers.Id = @p0",
                r => new Customer
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAsync(queryOptions);

            Assert.False(repo.CacheUsed);

            await repo.FindAsync(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAsync(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task FindAllAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(queryOptions);

            Assert.False(repo.CacheUsed);

            await repo.FindAllAsync(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.FindAllAsync(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task CountAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.CountAsync(queryOptions);

            Assert.False(repo.CacheUsed);

            await repo.CountAsync(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.CountAsync(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ExistsAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(queryOptions);

            Assert.False(repo.CacheUsed);

            await repo.ExistsAsync(queryOptions);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ExistsAsync(queryOptions);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task ToDictionaryAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(queryOptions, x => x.Id);

            Assert.False(repo.CacheUsed);

            await repo.ToDictionaryAsync(queryOptions, x => x.Id);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.ToDictionaryAsync(queryOptions, x => x.Id);

            Assert.False(repo.CacheUsed);
        }

        [Fact]
        public async Task GroupByAsync()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

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
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var repo = new Repository<Customer>(options);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == 0);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);

            await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key);

            Assert.False(repo.CacheUsed);
        }
    }
}
