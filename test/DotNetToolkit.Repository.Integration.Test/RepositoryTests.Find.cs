namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Queries.Strategies;
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

        private static void TestFind(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
                entities.Add(new Customer { Name = "Random Name " + i });
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
                entities.Add(new Customer { Name = "Random Name " + i });
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
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(repo.Find(key1, key2));
            Assert.Null(repo.Find(key1, key2, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Find(key1, randomKey));
            Assert.Null(repo.Find(key1, randomKey, fetchStrategy));
            Assert.NotNull(repo.Find(key1, key2));
            Assert.NotNull(repo.Find(key1, key2, fetchStrategy));
        }

        private static void TestFindWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(repo.Find(key1, key2, key3));
            Assert.Null(repo.Find(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Find(key1, key2, randomKey));
            Assert.Null(repo.Find(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(repo.Find(key1, key2, key3));
            Assert.NotNull(repo.Find(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestFindAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
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
                entities.Add(new Customer { Name = "Random Name " + i });
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
                entities.Add(new Customer { Name = "Random Name " + i });
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
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(await repo.FindAsync(key1, key2));
            Assert.Null(await repo.FindAsync(key1, key2, fetchStrategy));

            await repo.AddAsync(entity);

            Assert.Null(await repo.FindAsync(key1, randomKey));
            Assert.Null(await repo.FindAsync(key1, randomKey, fetchStrategy));
            Assert.NotNull(await repo.FindAsync(key1, key2));
            Assert.NotNull(await repo.FindAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestFindWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(await repo.FindAsync(key1, key2, key3));
            Assert.Null(await repo.FindAsync(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(await repo.FindAsync(key1, key2, randomKey));
            Assert.Null(await repo.FindAsync(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(await repo.FindAsync(key1, key2, key3));
            Assert.NotNull(await repo.FindAsync(key1, key2, key3, fetchStrategy));
        }
    }
}
