namespace DotNetToolkit.Repository.Integration.Test.Service
{
    using Data;
    using Query;
    using Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void GetGroupBy()
        {
            ForAllServiceFactories(TestGetGroupBy);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsAscending()
        {
            ForAllServiceFactories(TestGetGroupByWithSortingOptionsAscending);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsDescending()
        {
            ForAllServiceFactories(TestGetGroupByWithSortingOptionsDescending);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortAscending()
        {
            ForAllServiceFactories(TestGetGroupByWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortDescending()
        {
            ForAllServiceFactories(TestGetGroupByWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GetGroupByAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByAsync);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsAscendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsDescendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortAscendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortDescendingAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByWithPagingOptionsSortDescendingAsync);
        }

        private static void TestGetGroupBy(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGetGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));

            service.Create(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));
        }

        private static void TestGetGroupByWithSortingOptionsAscending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetGroupBy(options, y => y.Name, (key, g) => key).Result.First());
            Assert.Equal("Random Name 2", service.GetGroupBy(y => y.Name, (key, g) => key).First());
        }

        private static void TestGetGroupByWithSortingOptionsDescending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetGroupBy(y => y.Name, (key, g) => key).First());
            Assert.Equal("Random Name 1", service.GetGroupBy(options, y => y.Name, (key, g) => key).Result.First());
        }

        private static void TestGetGroupByWithPagingOptionsSortAscending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);

            var queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
        }

        private static void TestGetGroupByWithPagingOptionsSortDescending(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = service.GetGroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
        }

        private static async Task TestGetGroupByAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGetGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));

            await service.CreateAsync(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));
        }

        private static async Task TestGetGroupByWithSortingOptionsAscendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2",
                (await service.GetGroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
            Assert.Equal("Random Name 2", (await service.GetGroupByAsync(y => y.Name, (key, g) => key)).First());
        }

        private static async Task TestGetGroupByWithSortingOptionsDescendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer {Name = "Random Name 2"},
                new Customer {Name = "Random Name 1"}
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetGroupByAsync(y => y.Name, (key, g) => key)).First());
            Assert.Equal("Random Name 1",
                (await service.GetGroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
        }

        private static async Task TestGetGroupByWithPagingOptionsSortAscendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);

            var queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
        }

        private static async Task TestGetGroupByWithPagingOptionsSortDescendingAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Name = "Random Name " + i});
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = await service.GetGroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
        }
    }
}