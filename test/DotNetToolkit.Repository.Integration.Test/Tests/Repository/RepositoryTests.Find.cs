namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using Data;
    using Query;
    using Query.Strategies;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Find()
        {
            ForAllRepositoryFactories(TestFind);
        }

        [Fact]
        public void FindWithId()
        {
            ForAllRepositoryFactories(TestFindWithId);
        }

        [Fact]
        public void FindWithSortingOptionsAscending()
        {
            ForAllRepositoryFactories(TestFindWithSortingOptionsAscending);
        }

        [Fact]
        public void FindWithSortingOptionsDescending()
        {
            ForAllRepositoryFactories(TestFindWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAll()
        {
            ForAllRepositoryFactories(TestFindAll);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscending()
        {
            ForAllRepositoryFactories(TestFindAllWithSortingOptionsAscending);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescending()
        {
            ForAllRepositoryFactories(TestFindAllWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscending()
        {
            ForAllRepositoryFactories(TestFindAllWithPagingOptionsSortAscending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescending()
        {
            ForAllRepositoryFactories(TestFindAllWithPagingOptionsSortDescending);
        }

        [Fact]
        public void FindWithTwoCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestFindWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void FindWithThreeCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestFindWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void FindWithNavigationProperty_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindWithNavigationProperty_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindWithNavigationPropertyByKey_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindWithNavigationPropertyByKey_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindWithNavigationPropertyByCompositeKey_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindWithNavigationPropertyByCompositeKey_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindAllWithNavigationProperty_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindAllWithNavigationProperty_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindWithNavigationProperty_OneToManyRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindWithNavigationProperty_OneToManyRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindAllWithNavigationProperty_OneToManyRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactories(TestFindAllWithNavigationProperty_OneToManyRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAsync);
        }

        [Fact]
        public void FindWithIdAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindWithIdAsync);
        }

        [Fact]
        public void FindWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindWithSortingOptionsDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void FindAllAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAllAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAllWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAllWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAllWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAllWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void FindWithTwoCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void FindWithThreeCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindWithThreeCompositePrimaryKeyAsync);
        }

        [Fact]
        public void FindWithNavigationPropertyByKeyAsync_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactoriesAsync(TestFindWithNavigationPropertyByKeyAsync_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindWithNavigationPropertyAsync_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactoriesAsync(TestFindWithNavigationPropertyAsync_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindAllWithNavigationPropertyAsync_OneToOneRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactoriesAsync(TestFindAllWithNavigationPropertyAsync_OneToOneRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindWithNavigationPropertyAsync_OneToManyRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactoriesAsync(TestFindWithNavigationPropertyAsync_OneToManyRelationship, ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void FindAllWithNavigationPropertyAsync_OneToManyRelationship()
        {
            // currently not working for hibernate
            ForAllRepositoryFactoriesAsync(TestFindAllWithNavigationPropertyAsync_OneToManyRelationship, ContextProviderType.AzureStorageBlob);
        }

        private static void TestFind(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };

            Assert.Null(repo.Find(x => x.Name.Equals(name)));
            Assert.Null(repo.Find(options));
            Assert.Null(repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(repo.Find<string>(options, x => x.Name));

            repo.Add(entity);

            Assert.NotNull(repo.Find(x => x.Name.Equals(name)));
            Assert.NotNull(repo.Find(options));
            Assert.Equal(name, repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, repo.Find<string>(options, x => x.Name));
        }

        private static void TestFindWithId(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.Null(repo.Find(id));

            repo.Add(entity);

            Assert.NotNull(repo.Find(id));
        }

        private static void TestFindWithSortingOptionsDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null(repo.Find(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(repo.Find(options)?.Name);
            Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(repo.Find<string>(options, x => x.Name));

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.Find(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 2", repo.Find(options).Name);
            Assert.Equal("Random Name 2", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", repo.Find<string>(options, x => x.Name));
        }

        private static void TestFindWithSortingOptionsAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null(repo.Find(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(repo.Find(options)?.Name);
            Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(repo.Find<string>(options, x => x.Name));

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.Find(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 1", repo.Find(options).Name);
            Assert.Equal("Random Name 2", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", repo.Find<string>(options, x => x.Name));
        }

        private static void TestFindAll(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);
            var entity = new Customer { Id = 1, Name = name };

            Assert.Empty(repo.FindAll());
            Assert.Empty(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Empty(repo.FindAll(options).Result);
            Assert.Empty(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(repo.FindAll<string>(x => x.Name));
            Assert.Empty(repo.FindAll<string>(options, x => x.Name).Result);

            repo.Add(entity);

            Assert.Single(repo.FindAll());
            Assert.Single(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Single(repo.FindAll(options).Result);
            Assert.Single(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(repo.FindAll<string>(x => x.Name));
            Assert.Single(repo.FindAll<string>(options, x => x.Name).Result);
        }

        private static void TestFindAllWithSortingOptionsDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null(repo.FindAll().FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(options, x => x.Name).Result.FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 2", repo.FindAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", repo.FindAll<string>(options, x => x.Name).Result.First());

            Assert.Equal(1, repo.FindAll().First().Id);
            Assert.Equal(1, repo.FindAll(options).Result.First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => x.Id).First());
            Assert.Equal(1, repo.FindAll<int>(options, x => x.Id).Result.First());

            options = new QueryOptions<Customer>().OrderByDescending(x => x.Name).OrderByDescending(x => x.Id);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 2", repo.FindAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", repo.FindAll<string>(options, x => x.Name).Result.First());

            Assert.Equal(1, repo.FindAll().First().Id);
            Assert.Equal(3, repo.FindAll(options).Result.First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => x.Id).First());
            Assert.Equal(3, repo.FindAll<int>(options, x => x.Id).Result.First());
        }

        private static void TestFindAllWithSortingOptionsAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null(repo.FindAll().FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(options, x => x.Name).Result.FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 1", repo.FindAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", repo.FindAll<string>(options, x => x.Name).Result.First());

            options = new QueryOptions<Customer>().OrderBy(x => x.Name).OrderBy(x => x.Id);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 1", repo.FindAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", repo.FindAll<string>(options, x => x.Name).Result.First());
        }

        private static void TestFindAllWithPagingOptionsSortAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            repo.Add(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var queryResult = repo.FindAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestFindAllWithPagingOptionsSortDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            repo.Add(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);
            var queryResult = repo.FindAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = repo.FindAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestFindWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(repo.Find(key1, key2));
            Assert.Null(repo.Find(key1, key2, fetchStrategy));

            repo.Add(entity);

            Assert.NotNull(repo.Find(key1, key2));
            Assert.NotNull(repo.Find(key1, key2, fetchStrategy));
        }

        private static void TestFindWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(repo.Find(key1, key2, key3));
            Assert.Null(repo.Find(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.NotNull(repo.Find(key1, key2, key3));
            Assert.NotNull(repo.Find(key1, key2, key3, fetchStrategy));
        }

        private static void TestFindWithNavigationPropertyByKey_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
        {
            var addressRepo = repoFactory.Create<CustomerAddress>();
            var customerRepo = repoFactory.Create<Customer>();
            var fetchStrategy = new FetchQueryStrategy<Customer>()
                .Fetch(x => x.Address);

            var entity = new Customer
            {
                Name = "Random Name",
            };

            customerRepo.Add(entity);

            var address = new CustomerAddress
            {
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = entity.Id
            };

            Assert.Null(customerRepo.Find(entity.Id).Address);
            Assert.Null(customerRepo.Find(entity.Id, fetchStrategy).Address);
            Assert.Null(customerRepo.Find(entity.Id, x => x.Address).Address);
            Assert.Null(customerRepo.Find(entity.Id, "Address").Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            addressRepo.Add(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            Assert.Null(customerRepo.Find(entity.Id).Address);

            TestCustomerAddress(address, customerRepo.Find(entity.Id, fetchStrategy).Address);
            TestCustomerAddress(address, customerRepo.Find(entity.Id, x => x.Address).Address);
            TestCustomerAddress(address, customerRepo.Find(entity.Id, "Address").Address);
        }

        private static void TestFindAllWithNavigationProperty_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
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

            Assert.Null(customerRepo.FindAll(x => x.Id == entity.Id)?.FirstOrDefault()?.Address);
            Assert.Null(customerRepo.FindAll(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Result?.FirstOrDefault()?.Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            addressRepo.Add(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.FindAll(x => x.Id == entity.Id)?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, customerRepo.FindAll(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Result?.FirstOrDefault()?.Address);
        }

        private static void TestFindWithNavigationPropertyByCompositeKey_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
        {
            var addressRepo = repoFactory.Create<CustomerCompositeAddress, int, int>();
            var customerRepo = repoFactory.Create<CustomerWithCompositeAddress>();
            var customerFetchStrategy = new FetchQueryStrategy<CustomerWithCompositeAddress>().Fetch(x => x.Address);

            var entity = new CustomerWithCompositeAddress
            {
                Name = "Random Name"
            };

            customerRepo.Add(entity);

            var address = new CustomerCompositeAddress
            {
                Street = "Street",
                City = "New City",
                State = "ST",
                CustomerId = entity.Id
            };

            Assert.Null(customerRepo.Find(entity.Id).Address);
            Assert.Null(customerRepo.Find(entity.Id, customerFetchStrategy).Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            addressRepo.Add(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            Assert.Null(customerRepo.Find(entity.Id).Address);

            TestCustomerAddress(address, customerRepo.Find(entity.Id, customerFetchStrategy).Address);
        }

        private static void TestFindWithNavigationProperty_OneToManyRelationship(IRepositoryFactory repoFactory)
        {
            var addressRepo = repoFactory.Create<CustomerAddressWithMultipleAddresses>();
            var customerRepo = repoFactory.Create<CustomerWithMultipleAddresses>();
            var customerFetchStrategy = new FetchQueryStrategy<CustomerWithMultipleAddresses>().Fetch(x => x.Addresses);
            var queryOptions = new QueryOptions<CustomerWithMultipleAddresses>();
            var optionsWithFetchStrategy = new QueryOptions<CustomerWithMultipleAddresses>().Include(customerFetchStrategy);

            var entity = new CustomerWithMultipleAddresses
            {
                Name = "Random Name",
            };

            customerRepo.Add(entity);

            var addresses = new List<CustomerAddressWithMultipleAddresses>();

            for (int i = 0; i < 5; i++)
            {
                addresses.Add(new CustomerAddressWithMultipleAddresses()
                {
                    Street = $"Street {i}",
                    City = $"New City {i}",
                    State = $"ST {i}",
                    CustomerId = entity.Id
                });
            }

            Assert.Empty(customerRepo.Find(entity.Id)?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty(customerRepo.Find(queryOptions.SatisfyBy(x => x.Id == entity.Id))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty(customerRepo.Find(entity.Id, customerFetchStrategy)?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty(customerRepo.Find(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            addressRepo.Add(addresses);

            foreach (var address in addresses)
            {
                address.Customer = entity;
            }

            Assert.Empty(customerRepo.Find(entity.Id)?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty(customerRepo.Find(queryOptions.SatisfyBy(x => x.Id == entity.Id))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            TestCustomerAddress(addresses, customerRepo.Find(entity.Id, customerFetchStrategy).Addresses);
            TestCustomerAddress(addresses, customerRepo.Find(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id)).Addresses);
        }

        private static void TestFindAllWithNavigationProperty_OneToManyRelationship(IRepositoryFactory repoFactory)
        {
            var addressRepo = repoFactory.Create<CustomerAddressWithMultipleAddresses>();
            var customerRepo = repoFactory.Create<CustomerWithMultipleAddresses>();
            var customerFetchStrategy = new FetchQueryStrategy<CustomerWithMultipleAddresses>().Fetch(x => x.Addresses);
            var queryOptions = new QueryOptions<CustomerWithMultipleAddresses>();
            var optionsWithFetchStrategy = new QueryOptions<CustomerWithMultipleAddresses>().Include(customerFetchStrategy);

            var entity = new CustomerWithMultipleAddresses
            {
                Name = "Random Name"
            };

            customerRepo.Add(entity);

            var addresses = new List<CustomerAddressWithMultipleAddresses>();

            for (int i = 0; i < 5; i++)
            {
                addresses.Add(new CustomerAddressWithMultipleAddresses()
                {
                    Street = $"Street {i}",
                    City = $"New City {i}",
                    State = $"ST {i}",
                    CustomerId = entity.Id
                });
            }

            Assert.Empty(customerRepo.FindAll(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Result?.FirstOrDefault()?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty(customerRepo.FindAll(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id)).Result.FirstOrDefault().Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            addressRepo.Add(addresses);

            foreach (var address in addresses)
            {
                address.Customer = entity;
            }

            Assert.Empty(customerRepo.FindAll(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Result?.FirstOrDefault()?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            TestCustomerAddress(addresses, customerRepo.FindAll(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id)).Result.FirstOrDefault().Addresses);
        }

        private static async Task TestFindAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };

            Assert.Null(await repo.FindAsync(x => x.Name.Equals(name)));
            Assert.Null(await repo.FindAsync(options));
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(await repo.FindAsync<string>(options, x => x.Name));

            await repo.AddAsync(entity);

            Assert.NotNull(await repo.FindAsync(x => x.Name.Equals(name)));
            Assert.NotNull(await repo.FindAsync(options));
            Assert.Equal(name, await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, await repo.FindAsync<string>(options, x => x.Name));
        }

        private static async Task TestFindWithIdAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.Null(await repo.FindAsync(id));

            await repo.AddAsync(entity);

            Assert.NotNull(await repo.FindAsync(id));
        }

        private static async Task TestFindWithSortingOptionsDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await repo.FindAsync(options))?.Name);
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await repo.FindAsync<string>(options, x => x.Name));

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 2", (await repo.FindAsync(options)).Name);
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(options, x => x.Name));
        }

        private static async Task TestFindWithSortingOptionsAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await repo.FindAsync(options))?.Name);
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await repo.FindAsync<string>(options, x => x.Name));

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 1", (await repo.FindAsync(options)).Name);
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", await repo.FindAsync<string>(options, x => x.Name));
        }

        private static async Task TestFindAllAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);
            var entity = new Customer { Id = 1, Name = name };

            Assert.Empty(await repo.FindAllAsync());
            Assert.Empty(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Empty((await repo.FindAllAsync(options)).Result);
            Assert.Empty(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(await repo.FindAllAsync<string>(x => x.Name));
            Assert.Empty((await repo.FindAllAsync<string>(options, x => x.Name)).Result);

            await repo.AddAsync(entity);

            Assert.Single(await repo.FindAllAsync());
            Assert.Single(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Single((await repo.FindAllAsync(options)).Result);
            Assert.Single(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(await repo.FindAllAsync<string>(x => x.Name));
            Assert.Single((await repo.FindAllAsync<string>(options, x => x.Name)).Result);
        }

        private static async Task TestFindAllWithSortingOptionsDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null((await repo.FindAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(options, x => x.Name)).Result.First());

            Assert.Equal(1, (await repo.FindAllAsync()).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync(options)).Result.First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id)).First());
            Assert.Equal(1, (await repo.FindAllAsync<int>(options, x => x.Id)).Result.First());

            options = new QueryOptions<Customer>().OrderByDescending(x => x.Name).OrderByDescending(x => x.Id);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(options, x => x.Name)).Result.First());

            Assert.Equal(1, (await repo.FindAllAsync()).First().Id);
            Assert.Equal(3, (await repo.FindAllAsync(options)).Result.First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id)).First());
            Assert.Equal(3, (await repo.FindAllAsync<int>(options, x => x.Id)).Result.First());
        }

        private static async Task TestFindAllWithSortingOptionsAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null((await repo.FindAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await repo.FindAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(options, x => x.Name)).Result.First());

            options = new QueryOptions<Customer>().OrderBy(x => x.Name).OrderBy(x => x.Id);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await repo.FindAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(options, x => x.Name)).Result.First());
        }

        private static async Task TestFindAllWithPagingOptionsSortAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var queryResult = await repo.FindAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestFindAllWithPagingOptionsSortDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);
            var queryResult = await repo.FindAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = (await repo.FindAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestFindWithTwoCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(await repo.FindAsync(key1, key2));
            Assert.Null(await repo.FindAsync(key1, key2, fetchStrategy));

            await repo.AddAsync(entity);

            Assert.NotNull(await repo.FindAsync(key1, key2));
            Assert.NotNull(await repo.FindAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestFindWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(await repo.FindAsync(key1, key2, key3));
            Assert.Null(await repo.FindAsync(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.NotNull(await repo.FindAsync(key1, key2, key3));
            Assert.NotNull(await repo.FindAsync(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestFindWithNavigationPropertyByKeyAsync_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
        {
            var addressRepo = repoFactory.Create<CustomerAddress>();
            var customerRepo = repoFactory.Create<Customer>();
            var fetchStrategy = new FetchQueryStrategy<Customer>();

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
                CustomerId = entity.Id
            };

            Assert.Null((await customerRepo.FindAsync(entity.Id)).Address);
            Assert.Null((await customerRepo.FindAsync(entity.Id, fetchStrategy)).Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            addressRepo.Add(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            Assert.Null((await customerRepo.FindAsync(entity.Id)).Address);

            TestCustomerAddress(address, (await customerRepo.FindAsync(entity.Id, fetchStrategy)).Address);
        }

        private static void TestFindWithNavigationProperty_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
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

            Assert.Null(customerRepo.Find(x => x.Id == entity.Id).Address);
            Assert.Null(customerRepo.Find(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            addressRepo.Add(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, customerRepo.Find(x => x.Id == entity.Id).Address);
            TestCustomerAddress(address, customerRepo.Find(queryOptions.SatisfyBy(x => x.Id == entity.Id)).Address);
        }

        private static async Task TestFindWithNavigationPropertyAsync_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
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
                CustomerId = entity.Id
            };

            Assert.Null((await customerRepo.FindAsync(x => x.Id == entity.Id)).Address);
            Assert.Null((await customerRepo.FindAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            await addressRepo.AddAsync(address);

            // The customer needs to be added after for ef core, other wise it will try to add the customer to the database,
            // and will throw an exception because it already has been added
            if (providerType != ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.FindAsync(x => x.Id == entity.Id)).Address);
            TestCustomerAddress(address, (await customerRepo.FindAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Address);
        }

        private static async Task TestFindAllWithNavigationPropertyAsync_OneToOneRelationship(IRepositoryFactory repoFactory, ContextProviderType providerType)
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
                CustomerId = entity.Id
            };

            Assert.Null((await customerRepo.FindAllAsync(x => x.Id == entity.Id))?.FirstOrDefault()?.Address);
            Assert.Null((await customerRepo.FindAllAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Address);

            // The customer is required here, otherwise it will throw an exception for entity framework
            if (providerType == ContextProviderType.EntityFramework)
            {
                address.Customer = entity;
            }

            await addressRepo.AddAsync(address);

            address.Customer = entity;

            // for one to one, the navigation properties will be included automatically (no need to fetch)
            TestCustomerAddress(address, (await customerRepo.FindAllAsync(x => x.Id == entity.Id))?.FirstOrDefault()?.Address);
            TestCustomerAddress(address, (await customerRepo.FindAllAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Address);
        }

        private static async Task TestFindWithNavigationPropertyAsync_OneToManyRelationship(IRepositoryFactory repoFactory)
        {
            var addressRepo = repoFactory.Create<CustomerAddressWithMultipleAddresses>();
            var customerRepo = repoFactory.Create<CustomerWithMultipleAddresses>();
            var customerFetchStrategy = new FetchQueryStrategy<CustomerWithMultipleAddresses>().Fetch(x => x.Addresses);
            var queryOptions = new QueryOptions<CustomerWithMultipleAddresses>();
            var optionsWithFetchStrategy = new QueryOptions<CustomerWithMultipleAddresses>().Include(customerFetchStrategy);

            var entity = new CustomerWithMultipleAddresses
            {
                Name = "Random Name"
            };

            await customerRepo.AddAsync(entity);

            var addresses = new List<CustomerAddressWithMultipleAddresses>();

            for (int i = 0; i < 5; i++)
            {
                addresses.Add(new CustomerAddressWithMultipleAddresses()
                {
                    Street = $"Street {i}",
                    City = $"New City {i}",
                    State = $"ST {i}",
                    CustomerId = entity.Id
                });
            }

            Assert.Empty((await customerRepo.FindAsync(entity.Id))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty((await customerRepo.FindAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id)))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty((await customerRepo.FindAsync(entity.Id, customerFetchStrategy))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty((await customerRepo.FindAsync(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id)))?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            await addressRepo.AddAsync(addresses);

            foreach (var address in addresses)
            {
                address.Customer = entity;
            }

            Assert.Empty((await customerRepo.FindAsync(entity.Id)).Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty((await customerRepo.FindAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            TestCustomerAddress(addresses, (await customerRepo.FindAsync(entity.Id, customerFetchStrategy)).Addresses);
            TestCustomerAddress(addresses, (await customerRepo.FindAsync(optionsWithFetchStrategy.SatisfyBy(x => x.Id == entity.Id))).Addresses);
        }

        private static async Task TestFindAllWithNavigationPropertyAsync_OneToManyRelationship(IRepositoryFactory repoFactory)
        {
            var addressRepo = repoFactory.Create<CustomerAddressWithMultipleAddresses>();
            var customerRepo = repoFactory.Create<CustomerWithMultipleAddresses>();
            var customerFetchStrategy = new FetchQueryStrategy<CustomerWithMultipleAddresses>().Fetch(x => x.Addresses);
            var queryOptions = new QueryOptions<CustomerWithMultipleAddresses>();
            var optionsWithStrategies = new QueryOptions<CustomerWithMultipleAddresses>().Include(customerFetchStrategy);

            var entity = new CustomerWithMultipleAddresses
            {
                Name = "Random Name"
            };

            await customerRepo.AddAsync(entity);

            var addresses = new List<CustomerAddressWithMultipleAddresses>();

            for (int i = 0; i < 5; i++)
            {
                addresses.Add(new CustomerAddressWithMultipleAddresses()
                {
                    Street = $"Street {i}",
                    City = $"New City {i}",
                    State = $"ST {i}",
                    CustomerId = entity.Id
                });
            }

            Assert.Empty((await customerRepo.FindAllAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());
            Assert.Empty((await customerRepo.FindAllAsync(optionsWithStrategies.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Addresses ?? Enumerable.Empty<CustomerAddressWithMultipleAddresses>());

            await addressRepo.AddAsync(addresses);

            foreach (var address in addresses)
            {
                address.Customer = entity;
            }

            Assert.Null((await customerRepo.FindAllAsync(queryOptions.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Addresses);

            TestCustomerAddress(addresses, (await customerRepo.FindAllAsync(optionsWithStrategies.SatisfyBy(x => x.Id == entity.Id))).Result?.FirstOrDefault()?.Addresses);
        }

        private static void TestCustomerAddress(CustomerAddress expected, CustomerAddress actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);

            // The navigation property should have all the values mapped correctly
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);

            // The navigation property should have a key linking back to the main class (customer)
            Assert.NotEqual(0, actual.CustomerId);
            Assert.NotEqual(0, expected.CustomerId);
            Assert.Equal(expected.CustomerId, actual.CustomerId);

            // If the navigation property has also a navigation property linking back to the main class (customer),
            // then that navigation property should also be mapped correctly
            Assert.NotNull(expected.Customer);
            Assert.NotNull(actual.Customer);
            Assert.Equal(expected.Customer.Id, expected.Customer.Id);
            Assert.Equal(expected.Customer.Name, expected.Customer.Name);
        }

        private static void TestCustomerAddress(CustomerCompositeAddress expected, CustomerCompositeAddress actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotEqual(expected, actual);

            // The navigation property should have all the values mapped correctly
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);

            // The navigation property should have a key linking back to the main class (customer)
            Assert.NotEqual(0, actual.CustomerId);
            Assert.NotEqual(0, expected.CustomerId);
            Assert.Equal(expected.CustomerId, actual.CustomerId);

            // If the navigation property has also a navigation property linking back to the main class (customer),
            // then that navigation property should also be mapped correctly
            Assert.NotNull(expected.Customer);
            Assert.NotNull(actual.Customer);
            Assert.Equal(expected.Customer.Id, expected.Customer.Id);
            Assert.Equal(expected.Customer.Name, expected.Customer.Name);
        }

        private static void TestCustomerAddress(IEnumerable<CustomerAddressWithMultipleAddresses> expectedList, IEnumerable<CustomerAddressWithMultipleAddresses> actualList)
        {
            Assert.NotEmpty(expectedList);
            Assert.NotEmpty(actualList);
            Assert.NotEqual(expectedList, actualList);
            Assert.Equal(expectedList.Count(), actualList.Count());

            for (var i = 0; i < expectedList.Count(); i++)
            {
                var expected = expectedList.ElementAt(i);
                var actual = actualList.ElementAt(i);

                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.NotEqual(expected, actual);

                // The navigation property should have all the values mapped correctly
                Assert.Equal(expected.Street, actual.Street);
                Assert.Equal(expected.City, actual.City);
                Assert.Equal(expected.State, actual.State);

                // The navigation property should have a key linking back to the main class (customer)
                Assert.NotEqual(0, actual.CustomerId);
                Assert.NotEqual(0, expected.CustomerId);
                Assert.Equal(expected.CustomerId, actual.CustomerId);

                // If the navigation property has also a navigation property linking back to the main class (customer),
                // then that navigation property should also be mapped correctly
                Assert.NotNull(expected.Customer);
                Assert.NotNull(actual.Customer);
                Assert.Equal(expected.Customer.Id, expected.Customer.Id);
                Assert.Equal(expected.Customer.Name, expected.Customer.Name);
            }
        }
    }
}
