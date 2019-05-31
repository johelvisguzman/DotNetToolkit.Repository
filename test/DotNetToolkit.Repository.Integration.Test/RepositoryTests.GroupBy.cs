namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void GroupBy()
        {
            ForAllRepositoryFactories(TestGroupBy);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscending()
        {
            ForAllRepositoryFactories(TestGroupByWithSortingOptionsAscending);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescending()
        {
            ForAllRepositoryFactories(TestGroupByWithSortingOptionsDescending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscending()
        {
            ForAllRepositoryFactories(TestGroupByWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescending()
        {
            ForAllRepositoryFactories(TestGroupByWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByWithPagingOptionsSortDescendingAsync);
        }

        private static void TestGroupBy(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));

            repo.Add(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));
        }

        private static void TestGroupByWithSortingOptionsAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.GroupBy(options, y => y.Name, (key, g) => key).Result.First());
            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, (key, g) => key).First());
        }

        private static void TestGroupByWithSortingOptionsDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, (key, g) => key).First());
            Assert.Equal("Random Name 1", repo.GroupBy(options, y => y.Name, (key, g) => key).Result.First());
        }

        private static void TestGroupByWithPagingOptionsSortAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
        }

        private static void TestGroupByWithPagingOptionsSortDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);
            var queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = repo.GroupBy(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
        }

        private static async Task TestGroupByAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));

            await repo.AddAsync(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));
        }

        private static async Task TestGroupByWithSortingOptionsAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Name);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.GroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
            Assert.Equal("Random Name 2", (await repo.GroupByAsync(y => y.Name, (key, g) => key)).First());
        }

        private static async Task TestGroupByWithSortingOptionsDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().OrderBy(x => x.Name);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.GroupByAsync(y => y.Name, (key, g) => key)).First());
            Assert.Equal("Random Name 1", (await repo.GroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
        }

        private static async Task TestGroupByWithPagingOptionsSortAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);

            var queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
        }

        private static async Task TestGroupByWithPagingOptionsSortDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4));

            options = options.Page(2);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4));

            options = options.Page(3);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4));

            options = options.Page(4);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0));
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1));
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2));
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3));
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4));

            options = options.Page(5);

            queryResult = await repo.GroupByAsync(options, y => y.Name, (key, g) => key);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0));
        }
    }
}
