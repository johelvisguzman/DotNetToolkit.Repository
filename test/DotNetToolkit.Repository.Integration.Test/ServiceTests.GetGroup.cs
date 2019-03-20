namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void GetGroupBy()
        {
            ForAllUnitOfWorkFactories(TestGetGroupBy);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestGetGroupByWithSortingOptionsAscending);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestGetGroupByWithSortingOptionsDescending);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestGetGroupByWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestGetGroupByWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GetGroupByAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetGroupByAsync);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetGroupByWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GetGroupByWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetGroupByWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetGroupByWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GetGroupByWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetGroupByWithPagingOptionsSortDescendingAsync);
        }

        private static void TestGetGroupBy(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestGetGroupByWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestGetGroupByWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestGetGroupByWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestGetGroupByWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestGetGroupByAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestGetGroupByWithSortingOptionsAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestGetGroupByWithSortingOptionsDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestGetGroupByWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestGetGroupByWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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