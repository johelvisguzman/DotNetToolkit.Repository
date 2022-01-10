namespace DotNetToolkit.Repository.Integration.Test.CachingProvider
{
    using Caching.InMemory;
    using Configuration.Caching.Internal;
    using Data;
    using DotNetToolkit.Repository.Integration.Test.Helpers;
    using Fixtures;
    using Query;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryCachingTests : TestBase, IClassFixture<RepositoryCachingTestsFixture>
    {
        private ContextProviderType contextType;

        public RepositoryCachingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
#if NETFULL
            contextType = ContextProviderType.EntityFramework;
#else
            contextType = ContextProviderType.EntityFrameworkCore;
#endif
        }

        protected override void BeforeTest(CachingProviderType cachingProvider)
        {
            ClearCacheProvider(cachingProvider);
        }

        protected override void AfterTest(CachingProviderType cachingProvider)
        {
            ClearCacheProvider(cachingProvider);
        }

        [Fact]
        public void CacheEnabled()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseInMemoryCache()
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

            options = GetRepositoryOptionsBuilder(contextType)
                .Options;

            repo = new Repository<Customer>(options);

            Assert.False(repo.CacheEnabled);
        }

        [Fact]
        public void ClearCache()
        {
            var options = GetRepositoryOptionsBuilder(ContextProviderType.InMemory)
                .UseInMemoryCache()
                .Options;

            var customerRepo = new Repository<Customer>(options);
            var customer = new Customer { Name = "Random Name" };

            customerRepo.Add(customer);

            var customerAddressRepo = new Repository<CustomerAddress>(options);
            var customerAddress = new CustomerAddress() { CustomerId = customer.Id };

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
#if NETFULL
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestExecuteQuery);
#endif
        }

        [Fact]
        public void Find()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestFind);
        }

        [Fact]
        public void FindWithId()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestFindWithId);
        }

        [Fact]
        public void FindWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestFindWithOptions);
        }

        [Fact]
        public void FindAll()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestFindAll);
        }

        [Fact]
        public void FindAllWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestFindAllWithOptions);
        }

        [Fact]
        public void Count()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestCount);
        }

        [Fact]
        public void CountWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestCountWithOptions);
        }

        [Fact]
        public void Exists()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestExists);
        }

        [Fact]
        public void ExistsWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestExistsWithOptions);
        }

        [Fact]
        public void ToDictionary()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestToDictionary);
        }

        [Fact]
        public void ToDictionaryWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestToDictionaryWithOptions);
        }

        [Fact]
        public void GroupBy()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestGroupBy);
        }

        [Fact]
        public void GroupByWithOptions()
        {
            ForRepositoryFactoryWithAllCachingProviders(contextType, TestGroupByWithOptions);
        }

        [Fact]
        public void ExecuteQueryAsync()
        {
#if NETFULL
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestExecuteQueryAsync);
#endif
        }

        [Fact]
        public void FindAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestFindAsync);
        }

        [Fact]
        public void FindWithIdAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestFindWithIdAsync);
        }

        [Fact]
        public void FindWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestFindWithOptionsAsync);
        }

        [Fact]
        public void FindAllAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestFindAllAsync);
        }

        [Fact]
        public void FindAllWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestFindAllWithOptionsAsync);
        }

        [Fact]
        public void CountAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestCountAsync);
        }

        [Fact]
        public void CountWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestCountWithOptionsAsync);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestExistsAsync);
        }

        [Fact]
        public void ExistsWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestExistsWithOptionsAsync);
        }

        [Fact]
        public void ToDictionaryAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestToDictionaryAsync);
        }

        [Fact]
        public void ToDictionaryWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestToDictionaryWithOptionsAsync);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestGroupByAsync);
        }

        [Fact]
        public void GroupByWithOptionsAsync()
        {
            ForRepositoryFactoryWithAllCachingProvidersAsync(contextType, TestGroupByWithOptionsAsync);
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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            Assert.NotEmpty(repo.GroupBy(x => x.Id, z => z.Key));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(x => x.Id, z => z.Key));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.GroupBy(x => x.Id, z => z.Key));

            Assert.False(repo.CacheUsed);
        }

        private static void TestGroupByWithOptions(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            repo.Add(customer);

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, z => z.Key).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, z => z.Key).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(repo.GroupBy(queryOptions, x => x.Id, z => z.Key).Result);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

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

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, z => z.Key));

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, z => z.Key));

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty(await repo.GroupByAsync(x => x.Id, z => z.Key));

            Assert.False(repo.CacheUsed);
        }

        private static async Task TestGroupByWithOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = (Repository<Customer>)repoFactory.Create<Customer>();
            var customer = new Customer { Name = "Random Name" };

            await repo.AddAsync(customer);

            var queryOptions = new QueryOptions<Customer>().WithFilter(x => x.Id == customer.Id);

            Assert.True(repo.CacheEnabled);
            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, z => z.Key)).Result);

            Assert.False(repo.CacheUsed);

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, z => z.Key)).Result);

            Assert.True(repo.CacheUsed);

            repo.CacheEnabled = false;

            Assert.NotEmpty((await repo.GroupByAsync(queryOptions, x => x.Id, z => z.Key)).Result);

            Assert.False(repo.CacheUsed);
        }

        private static void ClearCacheProvider(CachingProviderType cachingProvider)
        {
#if NETFULL
            if (cachingProvider == CachingProviderType.Memcached)
            {
                MemcachedHelper.ClearDatabase("127.0.0.1", 11211);
            }
#endif
            if (cachingProvider == CachingProviderType.Redis)
            {
                RedisHelper.ClearDatabase("localhost", 0);
            }

            if (cachingProvider == CachingProviderType.Couchbase)
            {
                CouchbaseHelper.ClearDatabase("http://localhost:8091", "default", "password", "default");
            }
        }
    }
}