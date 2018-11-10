namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class ServiceTraitsTests : TestBase
    {
        public ServiceTraitsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Create()
        {
            ForAllUnitOfWorkFactories(TestCreate);
        }

        [Fact]
        public void CreateWithSeededIdForIdentity()
        {
            ForAllUnitOfWorkFactories(TestCreateWithSeededIdForIdentity);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentity()
        {
            ForAllUnitOfWorkFactories(TestCreateWithSeededIdForNoneIdentity);
        }

        [Fact]
        public void CreateRange()
        {
            ForAllUnitOfWorkFactories(TestCreateRange);
        }

        [Fact]
        public void Update()
        {
            ForAllUnitOfWorkFactories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllUnitOfWorkFactories(TestUpdateRange);
        }

        [Fact]
        public void Delete()
        {
            ForAllUnitOfWorkFactories(TestDelete);
        }

        [Fact]
        public void DeleteWithId()
        {
            ForAllUnitOfWorkFactories(TestDeleteWithId);
        }

        [Fact]
        public void TryDelete()
        {
            ForAllUnitOfWorkFactories(TestTryDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllUnitOfWorkFactories(TestDeleteRange);
        }

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
        public void GetCount()
        {
            ForAllUnitOfWorkFactories(TestGetCount);
        }

        [Fact]
        public void GetExists()
        {
            ForAllUnitOfWorkFactories(TestGetExists);
        }

        [Fact]
        public void GetExistsWithId()
        {
            ForAllUnitOfWorkFactories(TestGetExistsWithId);
        }

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
        public void CreateAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateAsync);
        }

        [Fact]
        public void CreateWithSeededIdForIdentityAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateWithSeededIdForIdentityAsync);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentityAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateWithSeededIdForNoneIdentityAsync);
        }

        [Fact]
        public void CreateRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateRangeAsync);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestUpdateRangeAsync);
        }

        [Fact]
        public void DeleteAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteWithIdAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithIdAsync);
        }

        [Fact]
        public void TryDeleteAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestTryDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteRangeAsync);
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
        public void GetCountAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetCountAsync);
        }

        [Fact]
        public void GetExistsAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetExistsAsync);
        }

        [Fact]
        public void GetExistsWithIdAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetExistsWithIdAsync);
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

        [Fact]
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        private static void TestCreate(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(service.GetExists(x => x.Name.Equals(name)));

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestCreateWithSeededIdForIdentity(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static void TestCreateWithSeededIdForNoneIdentity(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<CustomerWithNoIdentity>(uowFactory);

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(service.GetExists(key));
        }

        private static void TestCreateRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, service.GetCount());

            service.Create(entities);

            Assert.Equal(2, service.GetCount());
        }

        private static void TestUpdate(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            service.Create(entity);

            var entityInDb = service.Get(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            service.Update(entityInDb);

            Assert.True(service.GetExists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            service.Create(entities);

            var entitiesInDb = service.GetAll(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            service.Update(entitiesInDb);

            Assert.Equal(2, service.GetCount(x => x.Name.Equals(expectedName)));
        }

        private static void TestDelete(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));

            var entityInDb = service.Get(x => x.Name.Equals(name));

            service.Delete(entityInDb);

            Assert.False(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteWithId(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(service.GetExists(id));

            var ex = Assert.Throws<InvalidOperationException>(() => service.Delete(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            service.Create(entity);

            Assert.True(service.GetExists(id));

            service.Delete(id);

            Assert.False(service.GetExists(id));
        }

        private static void TestTryDelete(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(service.TryDelete(id));

            service.Create(entity);

            Assert.True(service.TryDelete(id));
        }

        private static void TestDeleteRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            service.Create(entities);

            Assert.Equal(2, service.GetCount());

            var entitiesInDb = service.GetAll(x => x.Name.Equals(name));

            service.Delete(entitiesInDb);

            Assert.Equal(0, service.GetCount());
        }

        private static void TestGet(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

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

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

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
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

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

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null(service.GetAll().FirstOrDefault()?.Name);
            Assert.Null(service.GetAll(options).Result.FirstOrDefault()?.Name);
            Assert.Null(service.GetAll<string>(x => x.Name).FirstOrDefault());
            Assert.Null(service.GetAll<string>(options, x => x.Name).Result.FirstOrDefault());

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetAll().First().Name);
            Assert.Equal("Random Name 1", service.GetAll(options).Result.First().Name);
            Assert.Equal("Random Name 2", service.GetAll<string>(x => x.Name).First());
            Assert.Equal("Random Name 1", service.GetAll<string>(options, x => x.Name).Result.First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
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

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(service.Get(key));
            Assert.Null(service.Get(key, fetchStrategy));

            service.Create(entity);

            Assert.NotNull(service.Get(key));
            Assert.NotNull(service.Get(key, fetchStrategy));
        }

        private static void TestGetCount(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, service.GetCount());
            Assert.Equal(0, service.GetCount(x => x.Name.Equals(name)));
            Assert.Equal(0, service.GetCount(options));

            service.Create(entity);

            Assert.Equal(1, service.GetCount());
            Assert.Equal(1, service.GetCount(x => x.Name.Equals(name)));
            Assert.Equal(1, service.GetCount(options));
        }

        private static void TestGetExists(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(service.GetExists(x => x.Name.Equals(name)));
            Assert.False(service.GetExists(options));

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));
            Assert.True(service.GetExists(options));
        }

        private static void TestGetExistsWithId(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(service.GetExists(id));

            service.Create(entity);

            Assert.True(service.GetExists(id));
        }

        private static void TestGetDictionary(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

            service.Create(entity);

            Assert.True(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestGetDictionaryWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static void TestGetDictionaryWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionary(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionary(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionary(options, y => y.Id, y => y.Name).Result.Contains(x)));
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

            Assert.False(expectedGetGroupByElementSelector.All(x => service.GetGroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x => service.GetGroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));

            service.Create(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x => service.GetGroupBy(y => y.Id, (key, g) => key).Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x => service.GetGroupBy(options, y => y.Id, (key, g) => key).Result.Contains(x.Key)));
        }

        private static void TestGetGroupByWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            service.Create(entities);

            Assert.Equal("Random Name 2", service.GetGroupBy(options, y => y.Name, (key, g) => key).Result.First());
            Assert.Equal("Random Name 2", service.GetGroupBy(y => y.Name, (key, g) => key).First());
        }

        private static void TestGetGroupByWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);

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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            service.Create(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

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

        private static async Task TestCreateAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestCreateWithSeededIdForIdentityAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static async Task TestCreateWithSeededIdForNoneIdentityAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<CustomerWithNoIdentity>(uowFactory);

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(await service.GetExistsAsync(key));
        }

        private static async Task TestCreateRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, await service.GetCountAsync());

            await service.CreateAsync(entities);

            Assert.Equal(2, await service.GetCountAsync());
        }

        private static async Task TestUpdateAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await service.CreateAsync(entity);

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await service.UpdateAsync(entityInDb);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await service.CreateAsync(entities);

            var entitiesInDb = await service.GetAllAsync(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            await service.UpdateAsync(entitiesInDb);

            Assert.Equal(2, await service.GetCountAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestDeleteAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            await service.DeleteAsync(entityInDb);

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteWithIdAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await service.GetExistsAsync(id));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(id));

            await service.DeleteAsync(id);

            Assert.False(await service.GetExistsAsync(id));
        }

        private static async Task TestTryDeleteAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await service.TryDeleteAsync(id));

            await service.CreateAsync(entity);

            Assert.True(await service.TryDeleteAsync(id));
        }

        private static async Task TestDeleteRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await service.CreateAsync(entities);

            Assert.Equal(2, await service.GetCountAsync());

            var entitiesInDb = await service.GetAllAsync(x => x.Name.Equals(name));

            await service.DeleteAsync(entitiesInDb);

            Assert.Equal(0, await service.GetCountAsync());
        }

        private static async Task TestGetAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null((await service.GetAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await service.GetAsync(options))?.Name);
            Assert.Null(await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await service.GetAsync<string>(options, x => x.Name));

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 2", (await service.GetAsync(options)).Name);
            Assert.Equal("Random Name 2", await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", await service.GetAsync<string>(options, x => x.Name));
        }

        private static async Task TestGetWithSortingOptionsAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null((await service.GetAsync(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await service.GetAsync(options))?.Name);
            Assert.Null(await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await service.GetAsync<string>(options, x => x.Name));

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAsync(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 1", (await service.GetAsync(options)).Name);
            Assert.Equal("Random Name 2", await service.GetAsync<string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", await service.GetAsync<string>(options, x => x.Name));
        }

        private static async Task TestGetAllAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

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
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

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

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

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
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null((await service.GetAllAsync()).FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync(options)).Result.FirstOrDefault()?.Name);
            Assert.Null((await service.GetAllAsync<string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await service.GetAllAsync<string>(options, x => x.Name)).Result.FirstOrDefault());

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetAllAsync()).First().Name);
            Assert.Equal("Random Name 1", (await service.GetAllAsync(options)).Result.First().Name);
            Assert.Equal("Random Name 2", (await service.GetAllAsync<string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await service.GetAllAsync<string>(options, x => x.Name)).Result.First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
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

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(await service.GetAsync(key));
            Assert.Null(await service.GetAsync(key, fetchStrategy));

            await service.CreateAsync(entity);

            Assert.NotNull(await service.GetAsync(key));
            Assert.NotNull(await service.GetAsync(key, fetchStrategy));
        }

        private static async Task TestGetCountAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, await service.GetCountAsync());
            Assert.Equal(0, await service.GetCountAsync(x => x.Name.Equals(name)));
            Assert.Equal(0, await service.GetCountAsync(options));

            await service.CreateAsync(entity);

            Assert.Equal(1, await service.GetCountAsync());
            Assert.Equal(1, await service.GetCountAsync(x => x.Name.Equals(name)));
            Assert.Equal(1, await service.GetCountAsync(options));
        }

        private static async Task TestGetExistsAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await service.GetExistsAsync(options));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await service.GetExistsAsync(options));
        }

        private static async Task TestGetExistsWithIdAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await service.GetExistsAsync(id));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(id));
        }

        private static async Task TestGetDictionaryAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

            await service.CreateAsync(entity);

            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestGetDictionaryWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
        }

        private static async Task TestGetDictionaryWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.False(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));

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
            Assert.True(expectedDictionary.All(x => service.GetDictionaryAsync(options, y => y.Id).Result.Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => service.GetDictionaryAsync(options, y => y.Id, y => y.Name).Result.Result.Contains(x)));
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

            Assert.False(expectedGetGroupByElementSelector.All(x => service.GetGroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x => service.GetGroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));

            await service.CreateAsync(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x => service.GetGroupByAsync(y => y.Id, (key, g) => key).Result.Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x => service.GetGroupByAsync(options, y => y.Id, (key, g) => key).Result.Result.Contains(x.Key)));
        }

        private static async Task TestGetGroupByWithSortingOptionsAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetGroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
            Assert.Equal("Random Name 2", (await service.GetGroupByAsync(y => y.Name, (key, g) => key)).First());
        }

        private static async Task TestGetGroupByWithSortingOptionsDescendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            await service.CreateAsync(entities);

            Assert.Equal("Random Name 2", (await service.GetGroupByAsync(y => y.Name, (key, g) => key)).First());
            Assert.Equal("Random Name 1", (await service.GetGroupByAsync(options, y => y.Name, (key, g) => key)).Result.First());
        }

        private static async Task TestGetGroupByWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);

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
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await service.CreateAsync(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

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

        private static void TestThrowsIfEntityPrimaryKeyTypesMismatch(IUnitOfWorkFactory uowFactory)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Service<Customer, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);
        }
    }
}