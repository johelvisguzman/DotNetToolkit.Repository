namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using FetchStrategies;
    using Queries;
    using Specifications;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class RepositoryTraitsTests : TestBase
    {
        [Fact]
        public void Add()
        {
            ForAllRepositories(TestAdd);
        }

        [Fact]
        public void AddRange()
        {
            ForAllRepositories(TestAddRange);
        }

        [Fact]
        public void Update()
        {
            ForAllRepositories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllRepositories(TestUpdateRange);
        }

        [Fact]
        public void Delete()
        {
            ForAllRepositories(TestDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllRepositories(TestDeleteRange);
        }

        [Fact]
        public void Find()
        {
            ForAllRepositories(TestFind);
        }

        [Fact]
        public void FindWithSortingOptionsAscending()
        {
            ForAllRepositories(TestFindWithSortingOptionsAscending);
        }

        [Fact]
        public void FindWithSortingOptionsDescending()
        {
            ForAllRepositories(TestFindWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAll()
        {
            ForAllRepositories(TestFindAll);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscending()
        {
            ForAllRepositories(TestFindAllWithSortingOptionsAscending);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescending()
        {
            ForAllRepositories(TestFindAllWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscending()
        {
            ForAllRepositories(TestFindAllWithPagingOptionsSortAscending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescending()
        {
            ForAllRepositories(TestFindAllWithPagingOptionsSortDescending);
        }

        [Fact]
        public void Get()
        {
            ForAllRepositories(TestGet);
        }

        [Fact]
        public void Count()
        {
            ForAllRepositories(TestCount);
        }

        [Fact]
        public void Exists()
        {
            ForAllRepositories(TestExists);
        }

        [Fact]
        public void ToDictionary()
        {
            ForAllRepositories(TestToDictionary);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscending()
        {
            ForAllRepositories(TestToDictionaryWithPagingOptionsSortAscending);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescending()
        {
            ForAllRepositories(TestToDictionaryWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GroupBy()
        {
            ForAllRepositories(TestGroupBy);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscending()
        {
            ForAllRepositories(TestGroupByWithSortingOptionsAscending);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescending()
        {
            ForAllRepositories(TestGroupByWithSortingOptionsDescending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscending()
        {
            ForAllRepositories(TestGroupByWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescending()
        {
            ForAllRepositories(TestGroupByWithPagingOptionsSortDescending);
        }

        [Fact]
        public void AddAsync()
        {
            ForAllRepositoriesAsync(TestAddAsync);
        }

        [Fact]
        public void AddRangeAsync()
        {
            ForAllRepositoriesAsync(TestAddRangeAsync);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllRepositoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllRepositoriesAsync(TestUpdateRangeAsync);
        }

        [Fact]
        public void DeleteAsync()
        {
            ForAllRepositoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllRepositoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void FindAsync()
        {
            ForAllRepositoriesAsync(TestFindAsync);
        }

        [Fact]
        public void FindWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoriesAsync(TestFindWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindWithSortingOptionsExecuteForAllRepositoriesAsync()
        {
            ForAllRepositoriesAsync(TestFindWithSortingOptionsExecuteForAllRepositoriesAsync);
        }

        [Fact]
        public void FindAllAsync()
        {
            ForAllRepositoriesAsync(TestFindAllAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoriesAsync(TestFindAllWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescendingAsync()
        {
            ForAllRepositoriesAsync(TestFindAllWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoriesAsync(TestFindAllWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoriesAsync(TestFindAllWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GetAsync()
        {
            ForAllRepositoriesAsync(TestGetAsync);
        }

        [Fact]
        public void CountAsync()
        {
            ForAllRepositoriesAsync(TestCountAsync);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForAllRepositoriesAsync(TestExistsAsync);
        }

        [Fact]
        public void ToDictionaryAsync()
        {
            ForAllRepositoriesAsync(TestToDictionaryAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoriesAsync(TestToDictionaryWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoriesAsync(TestToDictionaryWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForAllRepositoriesAsync(TestGroupByAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscendingAsync()
        {
            ForAllRepositoriesAsync(TestGroupByWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescendingAsync()
        {
            ForAllRepositoriesAsync(TestGroupByWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscendingAsync()
        {
            ForAllRepositoriesAsync(TestGroupByWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescendingAsync()
        {
            ForAllRepositoriesAsync(TestGroupByWithPagingOptionsSortDescendingAsync);
        }

        private static void TestAdd(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestAddRange(IRepository<Customer, int> repo)
        {
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

        private static void TestUpdate(IRepository<Customer, int> repo)
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            repo.Update(entityInDb);

            Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IRepository<Customer, int> repo)
        {
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

        private static void TestDelete(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            repo.Delete(entityInDb);

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteRange(IRepository<Customer, int> repo)
        {
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

        private static void TestFind(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Null(repo.Find(x => x.Name.Equals(name)));
            Assert.Null(repo.Find(spec));
            Assert.Null(repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(repo.Find<string>(spec, x => x.Name));

            repo.Add(entity);

            Assert.NotNull(repo.Find(x => x.Name.Equals(name)));
            Assert.NotNull(repo.Find(spec));
            Assert.Equal(name, repo.Find<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, repo.Find<string>(spec, x => x.Name));
        }

        private static void TestFindWithSortingOptionsDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            Assert.Null(repo.Find(x => x.Name.Contains("Random Name"), queryOptions)?.Name);
            Assert.Null(repo.Find(spec, queryOptions)?.Name);
            Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Null(repo.Find<string>(spec, x => x.Name, queryOptions));

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.Find(x => x.Name.Contains("Random Name"), queryOptions).Name);
            Assert.Equal("Random Name 2", repo.Find(spec, queryOptions).Name);
            Assert.Equal("Random Name 2", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Equal("Random Name 2", repo.Find<string>(spec, x => x.Name, queryOptions));
        }

        private static void TestFindWithSortingOptionsAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            Assert.Null(repo.Find(x => x.Name.Contains("Random Name"), queryOptions)?.Name);
            Assert.Null(repo.Find(spec, queryOptions)?.Name);
            Assert.Null(repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Null(repo.Find<string>(spec, x => x.Name, queryOptions));

            repo.Add(entities);

            Assert.Equal("Random Name 1", repo.Find(x => x.Name.Contains("Random Name"), queryOptions).Name);
            Assert.Equal("Random Name 1", repo.Find(spec, queryOptions).Name);
            Assert.Equal("Random Name 1", repo.Find<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Equal("Random Name 1", repo.Find<string>(spec, x => x.Name, queryOptions));
        }

        private static void TestFindAll(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Empty(repo.FindAll());
            Assert.Empty(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Empty(repo.FindAll(spec));
            Assert.Empty(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(repo.FindAll<string>(spec, x => x.Name));

            repo.Add(entity);

            Assert.Single(repo.FindAll());
            Assert.Single(repo.FindAll(x => x.Name.Equals(name)));
            Assert.Single(repo.FindAll(spec));
            Assert.Single(repo.FindAll<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(repo.FindAll<string>(spec, x => x.Name));
        }

        private static void TestFindAllWithSortingOptionsAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            Assert.Null(repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(spec, queryOptions).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(spec, x => x.Name, queryOptions).FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll(spec, queryOptions).First().Name);
            Assert.Equal("Random Name 2", repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).First());
            Assert.Equal("Random Name 2", repo.FindAll<string>(spec, x => x.Name, queryOptions).First());
        }

        private static void TestFindAllWithSortingOptionsDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            Assert.Null(repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll(spec, queryOptions).FirstOrDefault()?.Name);
            Assert.Null(repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).FirstOrDefault());
            Assert.Null(repo.FindAll<string>(spec, x => x.Name, queryOptions).FirstOrDefault());

            repo.Add(entities);

            Assert.Equal("Random Name 1", repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions).First().Name);
            Assert.Equal("Random Name 1", repo.FindAll(spec, queryOptions).First().Name);
            Assert.Equal("Random Name 1", repo.FindAll<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions).First());
            Assert.Equal("Random Name 1", repo.FindAll<string>(spec, x => x.Name, queryOptions).First());
        }

        private static void TestFindAllWithPagingOptionsSortAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestFindAllWithPagingOptionsSortDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.FindAll(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGet(IRepository<Customer, int> repo)
        {
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

        private static void TestCount(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Equal(0, repo.Count());
            Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(0, repo.Count(spec));

            repo.Add(entity);

            Assert.Equal(1, repo.Count());
            Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(1, repo.Count(spec));
        }

        private static void TestExists(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
            Assert.False(repo.Exists(spec));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
            Assert.True(repo.Exists(spec));
        }

        private static void TestToDictionary(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.ToDictionary(x => x.Id, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestGroupBy(IRepository<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroup.All(x => repo.GroupBy(y => y.Id).Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroup.All(x => repo.GroupBy(spec, y => y.Id).Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, y => y.Name).Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(spec, y => y.Id, y => y.Name).Select(y => y.Key).Contains(x.Key)));

            repo.Add(entities);

            Assert.True(expectedGroup.All(x => repo.GroupBy(y => y.Id).Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroup.All(x => repo.GroupBy(spec, y => y.Id).Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, y => y.Name).Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(spec, y => y.Id, y => y.Name).Select(y => y.Key).Contains(x.Key)));
        }

        private static void TestGroupByWithSortingOptionsAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            repo.Add(entities);

            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 2", repo.GroupBy(spec, y => y.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 2", repo.GroupBy(y => y.Name, x => x.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 2", repo.GroupBy(spec, y => y.Name, x => x.Name, queryOptions).First().Key);
        }

        private static void TestGroupByWithSortingOptionsDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            repo.Add(entities);

            Assert.Equal("Random Name 1", repo.GroupBy(y => y.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 1", repo.GroupBy(spec, y => y.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 1", repo.GroupBy(y => y.Name, x => x.Name, queryOptions).First().Key);
            Assert.Equal("Random Name 1", repo.GroupBy(spec, y => y.Name, x => x.Name, queryOptions).First().Key);
        }

        private static void TestGroupByWithPagingOptionsSortAscending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
        }

        private static void TestGroupByWithPagingOptionsSortDescending(IRepository<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            repo.Add(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 2;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 3;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 4;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 5;

            entitiesInDb = repo.GroupBy(y => y.Name, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestAddAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestAddRangeAsync(IRepositoryAsync<Customer, int> repo)
        {
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

        private static async Task TestUpdateAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await repo.UpdateAsync(entityInDb);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IRepositoryAsync<Customer, int> repo)
        {
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

        private static async Task TestDeleteAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            await repo.DeleteAsync(entityInDb);

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteRangeAsync(IRepositoryAsync<Customer, int> repo)
        {
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

        private static async Task TestFindAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Null(await repo.FindAsync(x => x.Name.Equals(name)));
            Assert.Null(await repo.FindAsync(spec));
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(await repo.FindAsync<string>(spec, x => x.Name));

            await repo.AddAsync(entity);

            Assert.NotNull(await repo.FindAsync(x => x.Name.Equals(name)));
            Assert.NotNull(await repo.FindAsync(spec));
            Assert.Equal(name, await repo.FindAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, await repo.FindAsync<string>(spec, x => x.Name));
        }

        private static async Task TestFindWithSortingOptionsAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions))?.Name);
            Assert.Null((await repo.FindAsync(spec, queryOptions))?.Name);
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Null(await repo.FindAsync<string>(spec, x => x.Name, queryOptions));

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions)).Name);
            Assert.Equal("Random Name 2", (await repo.FindAsync(spec, queryOptions)).Name);
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(spec, x => x.Name, queryOptions));
        }

        private static async Task TestFindWithSortingOptionsExecuteForAllRepositoriesAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            Assert.Null((await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions))?.Name);
            Assert.Null((await repo.FindAsync(spec, queryOptions))?.Name);
            Assert.Null(await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Null(await repo.FindAsync<string>(spec, x => x.Name, queryOptions));

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAsync(x => x.Name.Contains("Random Name"), queryOptions)).Name);
            Assert.Equal("Random Name 2", (await repo.FindAsync(spec, queryOptions)).Name);
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions));
            Assert.Equal("Random Name 2", await repo.FindAsync<string>(spec, x => x.Name, queryOptions));
        }

        private static async Task TestFindAllAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Empty(await repo.FindAllAsync());
            Assert.Empty(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Empty(await repo.FindAllAsync(spec));
            Assert.Empty(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(await repo.FindAllAsync<string>(spec, x => x.Name));

            await repo.AddAsync(entity);

            Assert.Single(await repo.FindAllAsync());
            Assert.Single(await repo.FindAllAsync(x => x.Name.Equals(name)));
            Assert.Single(await repo.FindAllAsync(spec));
            Assert.Single(await repo.FindAllAsync<string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(await repo.FindAllAsync<string>(spec, x => x.Name));
        }

        private static async Task TestFindAllWithSortingOptionsAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            Assert.Null((await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(spec, queryOptions)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(spec, queryOptions)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).First());
        }

        private static async Task TestFindAllWithSortingOptionsDescendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            Assert.Null((await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync(spec, queryOptions)).FirstOrDefault()?.Name);
            Assert.Null((await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).FirstOrDefault());
            Assert.Null((await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).FirstOrDefault());

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", (await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync(spec, queryOptions)).First().Name);
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name, queryOptions)).First());
            Assert.Equal("Random Name 2", (await repo.FindAllAsync<string>(spec, x => x.Name, queryOptions)).First());
        }

        private static async Task TestFindAllWithPagingOptionsSortAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestFindAllWithPagingOptionsSortDescendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.FindAllAsync(x => x.Name.Contains("Random Name"), queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetAsync(IRepositoryAsync<Customer, int> repo)
        {
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

        private static async Task TestCountAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.Equal(0, await repo.CountAsync());
            Assert.Equal(0, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(0, await repo.CountAsync(spec));

            await repo.AddAsync(entity);

            Assert.Equal(1, await repo.CountAsync());
            Assert.Equal(1, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(1, await repo.CountAsync(spec));
        }

        private static async Task TestExistsAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await repo.ExistsAsync(spec));

            repo.Add(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await repo.ExistsAsync(spec));
        }

        private static async Task TestToDictionaryAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            await repo.AddAsync(entity);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortDescendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            repo.Add(entities);

            var entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionary(spec, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionary(spec, y => y.Id, y => y.Name).Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            await repo.AddAsync(entities);

            var entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

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
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.ToDictionaryAsync(x => x.Id, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => repo.ToDictionaryAsync(spec, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => repo.ToDictionaryAsync(spec, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestGroupByAsync(IRepositoryAsync<Customer, int> repo)
        {
            const string name = "Random Name";

            var spec = new Specification<Customer>(x => x.Name.Equals(name));
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroup.All(x => repo.GroupByAsync(y => y.Id).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroup.All(x => repo.GroupByAsync(spec, y => y.Id).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, y => y.Name).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(spec, y => y.Id, y => y.Name).Result.Select(y => y.Key).Contains(x.Key)));

            await repo.AddAsync(entities);

            Assert.True(expectedGroup.All(x => repo.GroupByAsync(y => y.Id).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroup.All(x => repo.GroupByAsync(spec, y => y.Id).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, y => y.Name).Result.Select(y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(spec, y => y.Id, y => y.Name).Result.Select(y => y.Key).Contains(x.Key)));
        }

        private static async Task TestGroupByWithSortingOptionsAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name, true);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 2", repo.GroupByAsync(y => y.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 2", repo.GroupByAsync(spec, y => y.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 2", repo.GroupByAsync(y => y.Name, x => x.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 2", repo.GroupByAsync(spec, y => y.Name, x => x.Name, queryOptions).Result.First().Key);
        }

        private static async Task TestGroupByWithSortingOptionsDescendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var spec = new Specification<Customer>(x => x.Name.Contains("Random Name"));
            var queryOptions = new SortingOptions<Customer, string>(x => x.Name);

            await repo.AddAsync(entities);

            Assert.Equal("Random Name 1", repo.GroupByAsync(y => y.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 1", repo.GroupByAsync(spec, y => y.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 1", repo.GroupByAsync(y => y.Name, x => x.Name, queryOptions).Result.First().Key);
            Assert.Equal("Random Name 1", repo.GroupByAsync(spec, y => y.Name, x => x.Name, queryOptions).Result.First().Key);
        }

        private static async Task TestGroupByWithPagingOptionsSortAscendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestGroupByWithPagingOptionsSortDescendingAsync(IRepositoryAsync<Customer, int> repo)
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await repo.AddAsync(entities);

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 2;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 3;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 4;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            queryOptions.PageIndex = 5;

            entitiesInDb = await repo.GroupByAsync(y => y.Name, queryOptions);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }
    }
}