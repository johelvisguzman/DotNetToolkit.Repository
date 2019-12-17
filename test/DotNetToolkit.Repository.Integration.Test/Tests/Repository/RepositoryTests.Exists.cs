namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using Data;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Exists()
        {
            ForAllRepositoryFactories(TestExists);
        }

        [Fact]
        public void ExistsWithId()
        {
            ForAllRepositoryFactories(TestExistsWithId);
        }

        [Fact]
        public void ExistWithNavigationProperty_OneToOneRelationship()
        {
            ForAllRepositoryFactories(TestExistWithNavigationProperty_OneToOneRelationship, ContextProviderType.AzureStorageBlob, ContextProviderType.AzureStorageTable);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForAllRepositoryFactoriesAsync(TestExistsAsync);
        }

        [Fact]
        public void ExistsWithIdAsync()
        {
            ForAllRepositoryFactoriesAsync(TestExistsWithIdAsync);
        }

        [Fact]
        public void ExistWithNavigationPropertyAsync_OneToOneRelationship()
        {
            ForAllRepositoryFactoriesAsync(TestExistWithNavigationPropertyAsync_OneToOneRelationship, ContextProviderType.AzureStorageBlob, ContextProviderType.AzureStorageTable);
        }

        private static void TestExists(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer { Id = 1, Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
            Assert.False(repo.Exists(options));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
            Assert.True(repo.Exists(options));
        }

        private static void TestExistsWithId(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(repo.Exists(id));

            repo.Add(entity);

            Assert.True(repo.Exists(id));
        }

        private static void TestExistWithNavigationProperty_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
        {
            var addressRepo = repoFactory.Create<CustomerAddress>();
            var customerRepo = repoFactory.Create<Customer>();

            var queryOptions = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Name = "Random Name"
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = entity.Id
            };

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            Assert.False(customerRepo.Exists(queryOptions.SatisfyBy(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street)))));
            Assert.False(customerRepo.Exists(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street))));

            addressRepo.Add(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            Assert.True(customerRepo.Exists(queryOptions.SatisfyBy(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street)))));
            Assert.True(customerRepo.Exists(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street))));
        }

        private static async Task TestExistsAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer { Id = 1, Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await repo.ExistsAsync(options));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await repo.ExistsAsync(options));
        }

        private static async Task TestExistsWithIdAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await repo.ExistsAsync(id));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(id));
        }

        private static async Task TestExistWithNavigationPropertyAsync_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
        {
            var addressRepo = repoFactory.Create<CustomerAddress>();
            var customerRepo = repoFactory.Create<Customer>();

            var queryOptions = new QueryOptions<Customer>();

            var entity = new Customer
            {
                Name = "Random Name"
            };

            await customerRepo.AddAsync(entity);

            var address = new CustomerAddress
            {
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = entity.Id,
            };

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            Assert.False(await customerRepo.ExistsAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street)))));
            Assert.False(await customerRepo.ExistsAsync(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street))));

            await addressRepo.AddAsync(address);

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            Assert.True(await customerRepo.ExistsAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street)))));
            Assert.True(await customerRepo.ExistsAsync(x => x.Id == entity.Id && (x.Address != null && x.Address.Street.Equals(address.Street))));
        }
    }
}
