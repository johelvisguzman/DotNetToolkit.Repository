namespace DotNetToolkit.Repository.Integration.Test.Service
{
    using Data;
    using Query;
    using Query.Strategies;
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
            ForAllServiceFactories(TestGet);
        }

        [Fact]
        public void GetWithId()
        {
            ForAllServiceFactories(TestGetWithId);
        }

        [Fact]
        public void GetWithSortingOptionsAscending()
        {
            ForAllServiceFactories(TestGetWithSortingOptionsAscending);
        }

        [Fact]
        public void GetWithSortingOptionsDescending()
        {
            ForAllServiceFactories(TestGetWithSortingOptionsDescending);
        }

        [Fact]
        public void GetAll()
        {
            ForAllServiceFactories(TestGetAll);
        }

        [Fact]
        public void GetAllWithSortingOptionsAscending()
        {
            ForAllServiceFactories(TestGetAllWithSortingOptionsAscending);
        }

        [Fact]
        public void GetAllWithSortingOptionsDescending()
        {
            ForAllServiceFactories(TestGetAllWithSortingOptionsDescending);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortAscending()
        {
            ForAllServiceFactories(TestGetAllWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortDescending()
        {
            ForAllServiceFactories(TestGetAllWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKey()
        {
            ForAllServiceFactories(TestGetWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKey()
        {
            ForAllServiceFactories(TestGetWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void GetAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAsync);
        }

        [Fact]
        public void GetWithIdAsync()
        {
            ForAllServiceFactoriesAsync(TestGetWithIdAsync);
        }

        [Fact]
        public void GetWithSortingOptionsAscendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetWithSortingOptionsDescendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetAllAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAllAsync);
        }

        [Fact]
        public void GetAllWithSortingOptionsAscendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAllWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetAllWithSortingOptionsDescendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAllWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortAscendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAllWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GetAllWithPagingOptionsSortDescendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetAllWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKeyAsync()
        {
            ForAllServiceFactoriesAsync(TestGetWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKeyAsync()
        {
            ForAllServiceFactoriesAsync(TestGetWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestGet(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().WithFilter(x => x.Name.Equals(name)); ;
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

        private static void TestGetWithSortingOptionsDescending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name).WithFilter(x => x.Name.Contains("Random Name"));

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

        private static void TestGetWithSortingOptionsAscending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name).WithFilter(x => x.Name.Contains("Random Name"));

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

        private static void TestGetAll(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name);
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

        private static void TestGetAllWithSortingOptionsDescending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Id = 1, Name = "Random Name 2"},
                new Customer {Id = 2, Name = "Random Name 1"},
                new Customer {Id = 3, Name = "Random Name 2"}
            };

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name);

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

            options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name).WithSortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 2", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", service.GetAll<string>(options, x => x.Name).Result.First());

            Assert.Equal(1, service.GetAll().First().Id);
            Assert.Equal(3, service.GetAll(options).Result.First().Id);
            Assert.Equal(1, service.GetAll<int>(x => x.Id).First());
            Assert.Equal(3, service.GetAll<int>(options, x => x.Id).Result.First());
        }

        private static void TestGetAllWithSortingOptionsAscending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name);

            Assert.Null(service.GetAll().FirstOrDefault()?.Name);
            Assert.Null(service.GetAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(service.GetAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(service.GetAll<string>(options, x => x.Name).Result.FirstOrDefault());

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 1", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", service.GetAll<string>(options, x => x.Name).Result.First());

            options = new QueryOptions<Customer>().WithSortBy(x => x.Name).WithSortBy(x => x.Id);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 1", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", service.GetAll<string>(options, x => x.Name).Result.First());
        }

        private static void TestGetAllWithPagingOptionsSortAscending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Id).WithPage(1, 5);
            var queryResult = service.GetAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(2);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(3);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(4);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(5);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGetAllWithPagingOptionsSortDescending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Id).WithPage(1, 5);
            var queryResult = service.GetAll(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(2);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(3);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(4);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(5);

            entitiesInDb = service.GetAll(options).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGetWithId(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            int key = 1;

            var fetchStrategy = new FetchQueryStrategy<Customer>()
                .Fetch(x => x.Address);

            var entity = new Customer {Id = key, Name = "Random Name" };

            Assert.Null(service.Get(key));
            Assert.Null(service.Get(key, fetchStrategy));
            Assert.Null(service.Get(key, x => x.Address));
            Assert.Null(service.Get(key, "Address"));

            service.Create(entity);

            Assert.NotNull(service.Get(key));
            Assert.NotNull(service.Get(key, fetchStrategy));
            Assert.NotNull(service.Get(key, x => x.Address));
            Assert.NotNull(service.Get(key, "Address"));
        }

        private static void TestGetWithTwoCompositePrimaryKey(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2));
            Assert.Null(repo.Get(key1, key2, fetchStrategy));

            repo.Create(entity);

            Assert.NotNull(repo.Get(key1, key2));
            Assert.NotNull(repo.Get(key1, key2, fetchStrategy));
        }

        private static void TestGetWithThreeCompositePrimaryKey(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            
            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2, key3));
            Assert.Null(repo.Get(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.NotNull(repo.Get(key1, key2, key3));
            Assert.NotNull(repo.Get(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestGetAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().WithFilter(x => x.Name.Equals(name));
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

        private static async Task TestGetWithSortingOptionsDescendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name).WithFilter(x => x.Name.Contains("Random Name"));

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

        private static async Task TestGetWithSortingOptionsAscendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name).WithFilter(x => x.Name.Contains("Random Name"));

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

        private static async Task TestGetAllAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name);
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

        private static async Task TestGetAllWithSortingOptionsDescendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Id = 1, Name = "Random Name 2"},
                new Customer {Id = 2, Name = "Random Name 1"},
                new Customer {Id = 3, Name = "Random Name 2"}
            };

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name);

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

            options = new QueryOptions<Customer>().WithSortByDescending(x => x.Name).WithSortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            Assert.Equal(1, (await service.GetAllAsync()).First().Id);
            Assert.Equal(3, (await service.GetAllAsync(options)).Result.First().Id);
            Assert.Equal(1, (await service.GetAllAsync<int>(x => x.Id)).First());
            Assert.Equal(3, (await service.GetAllAsync<int>(options, x => x.Id)).Result.First());
        }

        private static async Task TestGetAllWithSortingOptionsAscendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Name);

            Assert.Null((await service.GetAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await service.GetAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            options = new QueryOptions<Customer>().WithSortBy(x => x.Name).WithSortBy(x => x.Id);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());
        }

        private static async Task TestGetAllWithPagingOptionsSortAscendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().WithSortBy(x => x.Id).WithPage(1, 5);
            var queryResult = await service.GetAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(2);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(3);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(4);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(5);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetAllWithPagingOptionsSortDescendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().WithSortByDescending(x => x.Id).WithPage(1, 5);
            var queryResult = await service.GetAllAsync(options);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(2);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(3);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(4);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.WithPage(5);

            entitiesInDb = (await service.GetAllAsync(options)).Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetWithIdAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            int key = 1;

            var fetchStrategy = new FetchQueryStrategy<Customer>()
                .Fetch(x => x.Address);

            var entity = new Customer {Id = key, Name = "Random Name" };

            Assert.Null(await service.GetAsync(key));
            Assert.Null(await service.GetAsync(key, fetchStrategy));
            Assert.Null(await service.GetAsync(key, x => x.Address));
            Assert.Null(await service.GetAsync(key, "Address"));

            await service.CreateAsync(entity);

            Assert.NotNull(await service.GetAsync(key));
            Assert.NotNull(await service.GetAsync(key, fetchStrategy));
            Assert.NotNull(await service.GetAsync(key, x => x.Address));
            Assert.NotNull(await service.GetAsync(key, "Address"));
        }

        private static async Task TestGetWithTwoCompositePrimaryKeyAsync(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2));
            Assert.Null(await repo.GetAsync(key1, key2, fetchStrategy));

            await repo.CreateAsync(entity);

            Assert.NotNull(await repo.GetAsync(key1, key2));
            Assert.NotNull(await repo.GetAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestGetWithThreeCompositePrimaryKeyAsync(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2, key3));
            Assert.Null(await repo.GetAsync(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.NotNull(await repo.GetAsync(key1, key2, key3));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3, fetchStrategy));
        }
    }
}