namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using FetchStrategies;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class RepositoryTraitsTests : TestBase
    {
        [Fact]
        public void Add()
        {
            ForAllRepositoryFactories(TestAdd);
        }

        [Fact]
        public void AddRange()
        {
            ForAllRepositoryFactories(TestAddRange);
        }

        [Fact]
        public void Update()
        {
            ForAllRepositoryFactories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllRepositoryFactories(TestUpdateRange);
        }

        [Fact]
        public void Delete()
        {
            ForAllRepositoryFactories(TestDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllRepositoryFactories(TestDeleteRange);
        }

        [Fact]
        public void Find()
        {
            ForAllRepositoryFactories(TestFind);
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
        public void Get()
        {
            ForAllRepositoryFactories(TestGet);
        }

        [Fact]
        public void Count()
        {
            ForAllRepositoryFactories(TestCount);
        }

        [Fact]
        public void Exists()
        {
            ForAllRepositoryFactories(TestExists);
        }

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
        public void AddAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddAsync);
        }

        [Fact]
        public void AddRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddRangeAsync);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllRepositoryFactoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestUpdateRangeAsync);
        }

        [Fact]
        public void DeleteAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void FindAsync()
        {
            ForAllRepositoryFactoriesAsync(TestFindAsync);
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
        public void GetAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGetAsync);
        }

        [Fact]
        public void CountAsync()
        {
            ForAllRepositoryFactoriesAsync(TestCountAsync);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForAllRepositoryFactoriesAsync(TestExistsAsync);
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

        private static void TestAdd(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestAddRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, repo.Count());

            repo.Add(entities);

            Assert.Equal(2, repo.Count());
        }

        private static void TestUpdate(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            repo.Update(entityInDb);

            Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            repo.Add(entities);

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            repo.Update(entitiesInDb);

            Assert.Equal(2, repo.Count(x => x.Name.Equals(expectedName)));
        }

        private static void TestDelete(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            repo.Delete(entityInDb);

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            repo.Add(entities);

            Assert.Equal(2, repo.Count());

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            repo.Delete(entitiesInDb);

            Assert.Equal(0, repo.Count());
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

        private static void TestFindWithSortingOptionsDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

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

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

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

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

            Assert.Empty(repo.FindAll());
            Assert.Empty(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Empty(repo.FindAll(options));
            Assert.Empty(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(repo.FindAll<string>(x => x.Name));
            Assert.Empty(repo.FindAll<string>(options, x => x.Name));

            repo.Add(entity);

            Assert.Single(repo.FindAll());
            Assert.Single(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Single(repo.FindAll(options));
            Assert.Single(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(repo.FindAll<string>(x => x.Name));
            Assert.Single(repo.FindAll<string>(options, x => x.Name));
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

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null(repo.FindAll().FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(options).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(options, x => x.Name).FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 2", repo.FindAll(options).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", repo.FindAll<string>(options, x => x.Name).First());

            Assert.Equal(1, repo.FindAll().First().Id);
            Assert.Equal(1, repo.FindAll(options).First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => x.Id).First());
            Assert.Equal(1, repo.FindAll<int>(options, x => x.Id).First());

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 2", repo.FindAll(options).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 2", repo.FindAll<string>(options, x => x.Name).First());

            Assert.Equal(1, repo.FindAll().First().Id);
            Assert.Equal(3, repo.FindAll(options).First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => x.Id).First());
            Assert.Equal(3, repo.FindAll<int>(options, x => x.Id).First());
        }

        private static void TestFindAllWithSortingOptionsAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null(repo.FindAll().FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(options).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(options, x => x.Name).FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 1", repo.FindAll(options).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", repo.FindAll<string>(options, x => x.Name).First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

            Assert.Equal("Random Name 2", repo.FindAll().First().Name);
            Assert.Equal("Random Name 1", repo.FindAll(options).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", repo.FindAll<string>(options, x => x.Name).First());
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

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = repo.FindAll(options);

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

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = repo.FindAll(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = repo.FindAll(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGet(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchStrategy<Customer>();
            fetchStrategy.Include(x => x.Address);

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(repo.Get(key));
            Assert.Null(repo.Get(key, fetchStrategy));

            repo.Add(entity);

            Assert.NotNull(repo.Get(key));
            Assert.NotNull(repo.Get(key, fetchStrategy));
        }

        private static void TestCount(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, repo.Count());
            Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(0, repo.Count(options));

            repo.Add(entity);

            Assert.Equal(1, repo.Count());
            Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(1, repo.Count(options));
        }

        private static void TestExists(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
            Assert.False(repo.Exists(options));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
            Assert.True(repo.Exists(options));
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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(2);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(3);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(4);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(5);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(2);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(3);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(4);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(5);

            entitiesInDb = repo.ToDictionary(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(options, y => y.Id, y => y.Name).Contains(x)));
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

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, y => y.Key).Contains(x.Key)));

            repo.Add(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, y => y.Key).Contains(x.Key)));
        }

        private static void TestGroupByWithSortingOptionsAscending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.GroupBy(options, y => y.Name, x => x.Key).First());
            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, x => x.Key).First());
        }

        private static void TestGroupByWithSortingOptionsDescending(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, x => x.Key).First());
            Assert.Equal("Random Name 1", repo.GroupBy(options, y => y.Name, x => x.Key).First());
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

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
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

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = repo.GroupBy(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestAddAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestAddRangeAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, await repo.CountAsync());

            await repo.AddAsync(entities);

            Assert.Equal(2, await repo.CountAsync());
        }

        private static async Task TestUpdateAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await repo.UpdateAsync(entityInDb);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await repo.AddAsync(entities);

            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            await repo.UpdateAsync(entitiesInDb);

            Assert.Equal(2, await repo.CountAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestDeleteAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

            await repo.DeleteAsync(entityInDb);

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteRangeAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await repo.AddAsync(entities);

            Assert.Equal(2, await repo.CountAsync());

            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

            await repo.DeleteAsync(entitiesInDb);

            Assert.Equal(0, await repo.CountAsync());
        }

        private static async Task TestFindAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

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

        private static async Task TestFindWithSortingOptionsDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

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

        private static async Task TestFindWithSortingOptionsAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

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

        private static async Task TestFindAllAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

            Assert.Empty(await repo.FindAllAsync());
            Assert.Empty(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Empty(await repo.FindAllAsync(options));
            Assert.Empty(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(await repo.FindAllAsync<string>(x => x.Name));
            Assert.Empty(await repo.FindAllAsync<string>(options, x => x.Name));

            await repo.AddAsync(entity);

            Assert.Single(await repo.FindAllAsync());
            Assert.Single(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Single(await repo.FindAllAsync(options));
            Assert.Single(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(await repo.FindAllAsync<string>(x => x.Name));
            Assert.Single(await repo.FindAllAsync<string>(options, x => x.Name));
        }

        private static async Task TestFindAllWithSortingOptionsDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null((await repo.FindAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(options)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(options, x => x.Name)).FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(options)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(options, x => x.Name)).First());

            Assert.Equal(1, (await repo.FindAllAsync()).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync(options)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id)).First());
            Assert.Equal(1, (await repo.FindAllAsync<int>(options, x => x.Id)).First());

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(options)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(options, x => x.Name)).First());

            Assert.Equal(1, (await repo.FindAllAsync()).First().Id);
            Assert.Equal(3, (await repo.FindAllAsync(options)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id)).First());
            Assert.Equal(3, (await repo.FindAllAsync<int>(options, x => x.Id)).First());
        }

        private static async Task TestFindAllWithSortingOptionsAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null((await repo.FindAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(options)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(options, x => x.Name)).FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await repo.FindAllAsync(options)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(options, x => x.Name)).First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await repo.FindAllAsync(options)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await repo.FindAllAsync<string>(options, x => x.Name)).First());
        }

        private static async Task TestFindAllWithPagingOptionsSortAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestFindAllWithPagingOptionsSortDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = await repo.FindAllAsync(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchStrategy<Customer>();
            fetchStrategy.Include(x => x.Address);

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(await repo.GetAsync(key));
            Assert.Null(await repo.GetAsync(key, fetchStrategy));

            await repo.AddAsync(entity);

            Assert.NotNull(await repo.GetAsync(key));
            Assert.NotNull(await repo.GetAsync(key, fetchStrategy));
        }

        private static async Task TestCountAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, await repo.CountAsync());
            Assert.Equal(0, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(0, await repo.CountAsync(options));

            await repo.AddAsync(entity);

            Assert.Equal(1, await repo.CountAsync());
            Assert.Equal(1, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(1, await repo.CountAsync(options));
        }

        private static async Task TestExistsAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await repo.ExistsAsync(options));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await repo.ExistsAsync(options));
        }

        private static async Task TestToDictionaryAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await repo.AddAsync(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await repo.AddAsync(entities);

            var entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await repo.AddAsync(entities);

            var entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

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
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            entitiesInDb = await repo.ToDictionaryAsync(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestGroupByAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, y => y.Key).Result.Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, y => y.Key).Result.Contains(x.Key)));

            await repo.AddAsync(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, y => y.Key).Result.Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, y => y.Key).Result.Contains(x.Key)));
        }

        private static async Task TestGroupByWithSortingOptionsAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.GroupByAsync(options, y => y.Name, x => x.Key)).First());
            Assert.Equal("Random Name 2", (await repo.GroupByAsync(y => y.Name, x => x.Key)).First());
        }

        private static async Task TestGroupByWithSortingOptionsDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.GroupByAsync(y => y.Name, x => x.Key)).First());
            Assert.Equal("Random Name 1", (await repo.GroupByAsync(options, y => y.Name, x => x.Key)).First());
        }

        private static async Task TestGroupByWithPagingOptionsSortAscendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestGroupByWithPagingOptionsSortDescendingAsync(IRepositoryFactoryAsync repoFactory)
        {
            var repo = repoFactory.CreateAsync<Customer>();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = await repo.GroupByAsync(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }
    }
}