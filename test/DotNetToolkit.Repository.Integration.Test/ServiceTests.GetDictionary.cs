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
        public void GetDictionary()
        {
            ForAllUnitOfWorkFactories(TestGetDictionary);
        }

        [Fact]
        public void GetDictionaryWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestGetDictionaryWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GetDictionaryWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestGetDictionaryWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GetDictionaryAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetDictionaryAsync);
        }

        [Fact]
        public void GetDictionaryWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetDictionaryWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GetDictionaryWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetDictionaryWithPagingOptionsSortDescendingAsync);
        }

        private static void TestGetDictionary(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Id = 1, Name = name};
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            service.Create(entity);

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestGetDictionaryWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Id = i + 1, Name = "Random Name " + i});
            }

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            service.Create(entities);

            var queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            queryResult = service.GetDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestGetDictionaryWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Id = i + 1, Name = "Random Name " + i});
            }

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            service.Create(entities);

            var queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            queryResult = service.GetDictionary(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            queryResult = service.GetDictionary(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(
                expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestGetDictionaryAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Id = 1, Name = name};
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await service.CreateAsync(entity);

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestGetDictionaryWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Id = i + 1, Name = "Random Name " + i});
            }

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await service.CreateAsync(entities);

            var queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(2);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(3);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(4);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(5);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestGetDictionaryWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer {Id = i + 1, Name = "Random Name " + i});
            }

            var options = new QueryOptions<Customer>().OrderByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await service.CreateAsync(entities);

            var queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(2);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(3);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(4);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            options = options.Page(5);

            queryResult = await service.GetDictionaryAsync(options, x => x.Id);

            Assert.Equal(21, queryResult.Total);

            entitiesInDb = queryResult.Result;

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x =>
                service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x =>
                service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }
    }
}