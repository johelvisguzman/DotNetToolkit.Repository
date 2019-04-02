namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Caching;
    using Data;
    using Extensions.Microsoft.Caching.Memory;
    using Factories;
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
                .Options;

            repo = new Repository<Customer>(options);

            Assert.False(repo.CacheEnabled);
        }

        [Fact]
        public void ClearCache()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseCachingProvider(new InMemoryCacheProvider())
                .Options;

            var customerRepo = new Repository<Customer>(options);
            var customer = new Customer { Name = "Random Name" };

            customerRepo.Add(customer);

            var customerAddressRepo = new Repository<CustomerAddress>(options);
            var customerAddress = new CustomerAddress() {CustomerId = customer.Id};

            customerAddressRepo.Add(customerAddress);

            Assert.True(customerRepo.CacheEnabled);
            Assert.False(customerRepo.CacheUsed);

            Assert.NotNull(customerRepo.Find(x => x.Id == customer.Id));
            Assert.False(customerRepo.CacheUsed);

            Assert.NotNull(customerAddressRepo.Find(x => x.Id == customerAddress.Id));
            Assert.False(customerAddressRepo.CacheUsed);

            Assert.NotNull(customerRepo.Find(x => x.Id == customer.Id));
            Assert.True(customerRepo.CacheUsed);

            Assert.NotNull(customerAddressRepo.Find(x => x.Id == customerAddress.Id));
            Assert.True(customerAddressRepo.CacheUsed);

            customerRepo.ClearCache();

            Assert.NotNull(customerRepo.Find(x => x.Id == customer.Id));
            Assert.False(customerRepo.CacheUsed);

            Assert.NotNull(customerAddressRepo.Find(x => x.Id == customerAddress.Id));
            Assert.True(customerAddressRepo.CacheUsed);

            customerAddressRepo.ClearCache();

            Assert.NotNull(customerAddressRepo.Find(x => x.Id == customerAddress.Id));
            Assert.False(customerAddressRepo.CacheUsed);
        }

        [Fact]
        public void ExecuteQuery()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestExecuteQuery);
        }

        [Fact]
        public void Find()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestFind);
        }

        [Fact]
        public void FindWithId()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestFindWithId);
        }

        [Fact]
        public void FindWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestFindWithOptions);
        }

        [Fact]
        public void FindAll()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestFindAll);
        }

        [Fact]
        public void FindAllWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestFindAllWithOptions);
        }

        [Fact]
        public void Count()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestCount);
        }

        [Fact]
        public void CountWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestCountWithOptions);
        }

        [Fact]
        public void Exists()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestExists);
        }

        [Fact]
        public void ExistsWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestExistsWithOptions);
        }

        [Fact]
        public void ToDictionary()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestToDictionary);
        }

        [Fact]
        public void ToDictionaryWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestToDictionaryWithOptions);
        }

        [Fact]
        public void GroupBy()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestGroupBy);
        }

        [Fact]
        public void GroupByWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(ContextProviderType.AdoNet, TestGroupByWithOptions);
        }

        [Fact]
        public void ExecuteQueryAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestExecuteQueryAsync);
        }

        [Fact]
        public void FindAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestFindAsync);
        }

        [Fact]
        public void FindWithIdAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestFindWithIdAsync);
        }

        [Fact]
        public void FindWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestFindWithOptionsAsync);
        }

        [Fact]
        public void FindAllAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestFindAllAsync);
        }

        [Fact]
        public void FindAllWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestFindAllWithOptionsAsync);
        }

        [Fact]
        public void CountAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestCountAsync);
        }

        [Fact]
        public void CountWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestCountWithOptionsAsync);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestExistsAsync);
        }

        [Fact]
        public void ExistsWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestExistsWithOptionsAsync);
        }

        [Fact]
        public void ToDictionaryAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestToDictionaryAsync);
        }

        [Fact]
        public void ToDictionaryWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestToDictionaryWithOptionsAsync);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestGroupByAsync);
        }

        [Fact]
        public void GroupByWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(ContextProviderType.AdoNet, TestGroupByWithOptionsAsync);
        }

        private static void TestExecuteQuery(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();

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

        private static void TestFind(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(repo.Find(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestFindWithId(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(repo.Find(customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestFindWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(repo.Find(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(repo.Find(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static void TestFindAll(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.FindAll(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.FindAll(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.FindAll(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestFindAllWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.FindAll(queryOptions).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.FindAll(queryOptions).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.FindAll(queryOptions).Result);

            Assert.False(repo.CacheUsed);
        }

        private static void TestCount(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.Equal(1, repo.Count(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.Equal(1, repo.Count(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.Equal(1, repo.Count(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestCountWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.Equal(1, repo.Count(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.Equal(1, repo.Count(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.Equal(1, repo.Count(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static void TestExists(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.True(repo.Exists(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.True(repo.Exists(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.True(repo.Exists(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestExistsWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.True(repo.Exists(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.True(repo.Exists(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.True(repo.Exists(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static void TestToDictionary(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.ToDictionary(x => x.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.ToDictionary(x => x.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.ToDictionary(x => x.Id));

            Assert.False(repo.CacheUsed);
        }

        private static void TestToDictionaryWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.ToDictionary(queryOptions, x => x.Id).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.ToDictionary(queryOptions, x => x.Id).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.ToDictionary(queryOptions, x => x.Id).Result);

            Assert.False(repo.CacheUsed);
        }

        private static void TestGroupBy(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(x => x.Id, (key, g) => key));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(x => x.Id, (key, g) => key));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.GroupBy(x => x.Id, (key, g) => key));

            Assert.False(repo.CacheUsed);
        }

        private static void TestGroupByWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, (key, g) => key).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, (key, g) => key).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, (key, g) => key).Result);

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestExecuteQueryAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            repo.ExecuteSqlCommand(@"
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
                    Name = r.GetString(1)
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
                    Name = r.GetString(1)
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
                    Name = r.GetString(1)
                });

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestFindAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(await repo.FindAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestFindWithIdAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(await repo.FindAsync(customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestFindWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.NotNull(await repo.FindAsync(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotNull(await repo.FindAsync(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestFindAllAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.FindAllAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.FindAllAsync(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(await repo.FindAllAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestFindAllWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.FindAllAsync(queryOptions)).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.FindAllAsync(queryOptions)).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty((await repo.FindAllAsync(queryOptions)).Result);

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestCountAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.Equal(1, await repo.CountAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.Equal(1, await repo.CountAsync(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.Equal(1, await repo.CountAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestCountWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.Equal(1, await repo.CountAsync(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.Equal(1, await repo.CountAsync(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.Equal(1, await repo.CountAsync(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestExistsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.True(await repo.ExistsAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);

            Assert.True(await repo.ExistsAsync(x => x.Id == customer.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.True(await repo.ExistsAsync(x => x.Id == customer.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestExistsWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.True(await repo.ExistsAsync(queryOptions));

            Assert.False(repo.CacheUsed);

            Assert.True(await repo.ExistsAsync(queryOptions));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.True(await repo.ExistsAsync(queryOptions));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestToDictionaryAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.ToDictionaryAsync(x => x.Id));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.ToDictionaryAsync(x => x.Id));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(await repo.ToDictionaryAsync(x => x.Id));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestToDictionaryWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.ToDictionaryAsync(queryOptions, x => x.Id)).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.ToDictionaryAsync(queryOptions, x => x.Id)).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty((await repo.ToDictionaryAsync(queryOptions, x => x.Id)).Result);

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestGroupByAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, (key, g) => key));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, (key, g) => key));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, (key, g) => key));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestGroupByWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().SatisfyBy(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key)).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key)).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, (key, g) => key)).Result);

            Assert.False(repo.CacheUsed);
        }
    }
}
