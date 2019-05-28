namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using Queries.Strategies;
    using Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void Get()
        {
            ForAllUnitOfWorkFactories(TestGet);
        }

        [Fact]
        public void GetWithId()
        {
            ForAllUnitOfWorkFactories(TestGetWithId);
        }

        [Fact]
        public void GetWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestGetWithSortingOptionsAscending);
        }

        [Fact]
        public void GetWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestGetWithSortingOptionsDescending);
        }

        [Fact]
        public void GetAll()
        {
            ForAllUnitOfWorkFactories(TestGetAll);
        }

        [Fact]
        public void GetAllWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestGetAllWithSortingOptionsAscending);
        }

        [Fact]
        public void GetAllWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestGetAllWithSortingOptionsDescending);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestGetAllWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestGetAllWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestGetWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestGetWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void GetAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAsync);
        }

        [Fact]
        public void GetWithIdAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithIdAsync);
        }

        [Fact]
        public void GetWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetAllAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAllAsync);
        }

        [Fact]
        public void GetAllWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAllWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetAllWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAllWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAllWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAllWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestGet(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Name = name};

            Assert.Null(service.Get(x => x.Name.Equals(name)));
            Assert.Null(service.Get(options));
            Assert.Null(service.Get<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(service.Get<string>(options, x => x.Name));

            service.Create(entity);

            Assert.NotNull(service.Get(x => x.Name.Equals(name)));
            Assert.NotNull(service.Get(options));
            Assert.Equal(name, service.Get<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, service.Get<string>(options, x => x.Name));
        }

        private static void TestGetWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null(service.Get(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(service.Get(options)?.Name);
            Assert.Null(service.Get<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(service.Get<string>(options, x => x.Name));

            service.Create(entities);

            Assert.Equal("Random Name 2", service.Get(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 2", service.Get(options).Name);
            Assert.Equal("Random Name 2", service.Get<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", service.Get<string>(options, x => x.Name));
        }

        private static void TestGetWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null(service.Get(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(service.Get(options)?.Name);
            Assert.Null(service.Get<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(service.Get<string>(options, x => x.Name));

            service.Create(entities);

            Assert.Equal("Random Name 2", service.Get(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 1", service.Get(options).Name);
            Assert.Equal("Random Name 2", service.Get<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", service.Get<string>(options, x => x.Name));
        }

        private static void TestGetAll(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);
            var entity = new Customer {Name = name};

            Assert.Empty(service.GetAll());
            Assert.Empty(service.GetAll(x => x.Name.Equals(name)));
            Assert.Empty(service.GetAll(options).Result);
            Assert.Empty(service.GetAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(service.GetAll<string>(x => x.Name));
            Assert.Empty(service.GetAll<string>(options, x => x.Name).Result);

            service.Create(entity);

            Assert.Single(service.GetAll());
            Assert.Single(service.GetAll(x => x.Name.Equals(name)));
            Assert.Single(service.GetAll(options).Result);
            Assert.Single(service.GetAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(service.GetAll<string>(x => x.Name));
            Assert.Single(service.GetAll<string>(options, x => x.Name).Result);
        }

        private static void TestGetAllWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Id = 1, Name = "Random Name 2"},
                new Customer {Id = 2, Name = "Random Name 1"},
                new Customer {Id = 3, Name = "Random Name 2"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null(service.GetAll().FirstOrDefault()?.Name);
            Assert.Null(service.GetAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(service.GetAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(service.GetAll<string>(options, x => x.Name).Result.FirstOrDefault());

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 2", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", service.GetAll<string>(options, x => x.Name).Result.First());

            Assert.Equal(1, service.GetAll().First().Id);
            Assert.Equal(1, service.GetAll(options).Result.First().Id);
            Assert.Equal(1, service.GetAll<int>(x => x.Id).First());
            Assert.Equal(1, service.GetAll<int>(options, x => x.Id).Result.First());

            options = new QueryOptions<Customer>().OrderByDescending(x => x.Name).OrderByDescending(x => x.Id);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 2", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", service.GetAll<string>(options, x => x.Name).Result.First());

            Assert.Equal(1, service.GetAll().First().Id);
            Assert.Equal(3, service.GetAll(options).Result.First().Id);
            Assert.Equal(1, service.GetAll<int>(x => x.Id).First());
            Assert.Equal(3, service.GetAll<int>(options, x => x.Id).Result.First());
        }

        private static void TestGetAllWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null(service.GetAll().FirstOrDefault()?.Name);
            Assert.Null(service.GetAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(service.GetAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(service.GetAll<string>(options, x => x.Name).Result.FirstOrDefault());

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 1", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", service.GetAll<string>(options, x => x.Name).Result.First());

            options = new QueryOptions<Customer>().OrderBy(x => x.Name).OrderBy(x => x.Id);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 1", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", service.GetAll<string>(options, x => x.Name).Result.First());
        }

        private static void TestGetAllWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var queryResult = service.GetAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGetAllWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);
            var queryResult = service.GetAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGetWithId(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchQueryStrategy<Customer>();
            fetchStrategy.Fetch(x => x.Address);

            var entity = new Customer {Id = key, Name = name};

            Assert.Null(service.Get(key));
            Assert.Null(service.Get(key, fetchStrategy));

            service.Create(entity);

            Assert.NotNull(service.Get(key));
            Assert.NotNull(service.Get(key, fetchStrategy));
        }

        private static void TestGetWithTwoCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2));
            Assert.Null(repo.Get(key1, key2, fetchStrategy));

            repo.Create(entity);

            Assert.Null(repo.Get(key1, randomKey));
            Assert.Null(repo.Get(key1, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2));
            Assert.NotNull(repo.Get(key1, key2, fetchStrategy));
        }

        private static void TestGetWithThreeCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2, key3));
            Assert.Null(repo.Get(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.Null(repo.Get(key1, key2, randomKey));
            Assert.Null(repo.Get(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2, key3));
            Assert.NotNull(repo.Get(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestGetAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Name = name};

            Assert.Null(await service.GetAsync(x => x.Name.Equals(name)));
            Assert.Null(await service.GetAsync(options));
            Assert.Null(await service.GetAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(await service.GetAsync<string>(options, x => x.Name));

            await service.CreateAsync(entity);

            Assert.NotNull(await service.GetAsync(x => x.Name.Equals(name)));
            Assert.NotNull(await service.GetAsync(options));
            Assert.Equal(name, await service.GetAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, await service.GetAsync<string>(options, x => x.Name));
        }

        private static async Task TestGetWithSortingOptionsDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null((await service.GetAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await service.GetAsync(options))?.Name);
            Assert.Null(await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await service.GetAsync<string>(options, x => x.Name));

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 2", (await service.GetAsync(options)).Name);
            Assert.Equal("Random Name 2",
                await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", await service.GetAsync<string>(options, x => x.Name));
        }

        private static async Task TestGetWithSortingOptionsAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null((await service.GetAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await service.GetAsync(options))?.Name);
            Assert.Null(await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await service.GetAsync<string>(options, x => x.Name));

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 1", (await service.GetAsync(options)).Name);
            Assert.Equal("Random Name 2",
                await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", await service.GetAsync<string>(options, x => x.Name));
        }

        private static async Task TestGetAllAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);
            var entity = new Customer {Name = name};

            Assert.Empty(await service.GetAllAsync());
            Assert.Empty(await service.GetAllAsync(x => x.Name.Equals(name)));
            Assert.Empty((await service.GetAllAsync(options)).Result);
            Assert.Empty(await service.GetAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(await service.GetAllAsync<string>(x => x.Name));
            Assert.Empty((await service.GetAllAsync<string>(options, x => x.Name)).Result);

            await service.CreateAsync(entity);

            Assert.Single(await service.GetAllAsync());
            Assert.Single(await service.GetAllAsync(x => x.Name.Equals(name)));
            Assert.Single((await service.GetAllAsync(options)).Result);
            Assert.Single(await service.GetAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(await service.GetAllAsync<string>(x => x.Name));
            Assert.Single((await service.GetAllAsync<string>(options, x => x.Name)).Result);
        }

        private static async Task TestGetAllWithSortingOptionsDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Id = 1, Name = "Random Name 2"},
                new Customer {Id = 2, Name = "Random Name 1"},
                new Customer {Id = 3, Name = "Random Name 2"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            Assert.Null((await service.GetAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await service.GetAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            Assert.Equal(1, (await service.GetAllAsync()).First().Id);
            Assert.Equal(1, (await service.GetAllAsync(options)).Result.First().Id);
            Assert.Equal(1, (await service.GetAllAsync<int>(x => x.Id)).First());
            Assert.Equal(1, (await service.GetAllAsync<int>(options, x => x.Id)).Result.First());

            options = new QueryOptions<Customer>().OrderByDescending(x => x.Name).OrderByDescending(x => x.Id);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            Assert.Equal(1, (await service.GetAllAsync()).First().Id);
            Assert.Equal(3, (await service.GetAllAsync(options)).Result.First().Id);
            Assert.Equal(1, (await service.GetAllAsync<int>(x => x.Id)).First());
            Assert.Equal(3, (await service.GetAllAsync<int>(options, x => x.Id)).Result.First());
        }

        private static async Task TestGetAllWithSortingOptionsAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            Assert.Null((await service.GetAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await service.GetAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            options = new QueryOptions<Customer>().OrderBy(x => x.Name).OrderBy(x => x.Id);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());
        }

        private static async Task TestGetAllWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var queryResult = await service.GetAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetAllWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);
            var queryResult = await service.GetAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetWithIdAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchQueryStrategy<Customer>();
            fetchStrategy.Fetch(x => x.Address);

            var entity = new Customer {Id = key, Name = name};

            Assert.Null(await service.GetAsync(key));
            Assert.Null(await service.GetAsync(key, fetchStrategy));

            await service.CreateAsync(entity);

            Assert.NotNull(await service.GetAsync(key));
            Assert.NotNull(await service.GetAsync(key, fetchStrategy));
        }

        private static async Task TestGetWithTwoCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2));
            Assert.Null(await repo.GetAsync(key1, key2, fetchStrategy));

            await repo.CreateAsync(entity);

            Assert.Null(await repo.GetAsync(key1, randomKey));
            Assert.Null(await repo.GetAsync(key1, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2));
            Assert.NotNull(await repo.GetAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestGetWithThreeCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2, key3));
            Assert.Null(await repo.GetAsync(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.Null(await repo.GetAsync(key1, key2, randomKey));
            Assert.Null(await repo.GetAsync(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3, fetchStrategy));
        }
    }
}