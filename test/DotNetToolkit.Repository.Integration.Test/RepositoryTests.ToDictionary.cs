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
        public void ToDictionary()
        {
            ForAllRepositoryFactories(TestToDictionary);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscending()
        {
            ForAllRepositoryFactories(TestToDictionaryWithPagingOptionsSortAscending);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescending()
        {
            ForAllRepositoryFactories(TestToDictionaryWithPagingOptionsSortDescending);
        }

        [Fact]
        public void ToDictionaryAsync()
        {
            ForAllRepositoryFactoriesAsync(TestToDictionaryAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestToDictionaryWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoryFactoriesAsync(TestToDictionaryWithPagingOptionsSortDescendingAsync);
        }

        private static void TestToDictionary(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            repo.Add(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            repo.Add(entities);

            var queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            repo.Add(entities);

            var queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            queryResult = repo.ToDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await repo.AddAsync(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortAscendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await repo.AddAsync(entities);

            var queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(2);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(3);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(4);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(5);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortDescendingAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await repo.AddAsync(entities);

            var queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            var entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(2);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(3);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(4);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Value.Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Value.Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Value.Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Value.Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);
            expectedDictionary.Add(entities[1].Id, entities[1]);
            expectedDictionary.Add(entities[2].Id, entities[2]);
            expectedDictionary.Add(entities[3].Id, entities[3]);
            expectedDictionary.Add(entities[4].Id, entities[4]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);
            expectedDictionaryByElementSelector.Add(entities[1].Id, entities[1].Name);
            expectedDictionaryByElementSelector.Add(entities[2].Id, entities[2].Name);
            expectedDictionaryByElementSelector.Add(entities[3].Id, entities[3].Name);
            expectedDictionaryByElementSelector.Add(entities[4].Id, entities[4].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(5);

            queryResult = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }
    }
}
