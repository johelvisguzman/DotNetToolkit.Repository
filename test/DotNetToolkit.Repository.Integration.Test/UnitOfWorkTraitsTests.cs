namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using FetchStrategies;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class UnitOfWorkTraitsTests : TestBase
    {
        [Fact]
        public void DisposeRollBackUnComittedChanges()
        {
            ForAllUnitOfWorkFactories(TestDisposeRollBackUnComittedChanges);
        }

        [Fact]
        public void Commit()
        {
            ForAllUnitOfWorkFactories(TestComit);
        }

        [Fact]
        public void ThrowsIfAlreadyComittedOrDisposed()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfAlreadyComittedOrDisposed);
        }

        [Fact]
        public void Add()
        {
            ForAllUnitOfWorkFactories(TestAdd);
        }

        [Fact]
        public void AddRange()
        {
            ForAllUnitOfWorkFactories(TestAddRange);
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
        public void DeleteRange()
        {
            ForAllUnitOfWorkFactories(TestDeleteRange);
        }

        [Fact]
        public void Find()
        {
            ForAllUnitOfWorkFactories(TestFind);
        }

        [Fact]
        public void FindWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestFindWithSortingOptionsAscending);
        }

        [Fact]
        public void FindWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestFindWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAll()
        {
            ForAllUnitOfWorkFactories(TestFindAll);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestFindAllWithSortingOptionsAscending);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestFindAllWithSortingOptionsDescending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestFindAllWithPagingOptionsSortAscending);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestFindAllWithPagingOptionsSortDescending);
        }

        [Fact]
        public void Get()
        {
            ForAllUnitOfWorkFactories(TestGet);
        }

        [Fact]
        public void Count()
        {
            ForAllUnitOfWorkFactories(TestCount);
        }

        [Fact]
        public void Exists()
        {
            ForAllUnitOfWorkFactories(TestExists);
        }

        [Fact]
        public void ToDictionary()
        {
            ForAllUnitOfWorkFactories(TestToDictionary);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestToDictionaryWithPagingOptionsSortAscending);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestToDictionaryWithPagingOptionsSortDescending);
        }

        [Fact]
        public void GroupBy()
        {
            ForAllUnitOfWorkFactories(TestGroupBy);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscending()
        {
            ForAllUnitOfWorkFactories(TestGroupByWithSortingOptionsAscending);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescending()
        {
            ForAllUnitOfWorkFactories(TestGroupByWithSortingOptionsDescending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscending()
        {
            ForAllUnitOfWorkFactories(TestGroupByWithPagingOptionsSortAscending);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescending()
        {
            ForAllUnitOfWorkFactories(TestGroupByWithPagingOptionsSortDescending);
        }

        [Fact]
        public void AddAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestAddAsync);
        }

        [Fact]
        public void AddRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestAddRangeAsync);
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
        public void DeleteRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void FindAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAsync);
        }

        [Fact]
        public void FindWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void FindAllAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAllAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAllWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void FindAllWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAllWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAllWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void FindAllWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestFindAllWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GetAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetAsync);
        }

        [Fact]
        public void CountAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCountAsync);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestExistsAsync);
        }

        [Fact]
        public void ToDictionaryAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestToDictionaryAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestToDictionaryWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void ToDictionaryWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestToDictionaryWithPagingOptionsSortDescendingAsync);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGroupByAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGroupByWithSortingOptionsAscendingAsync);
        }

        [Fact]
        public void GroupByWithSortingOptionsDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGroupByWithSortingOptionsDescendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortAscendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGroupByWithPagingOptionsSortAscendingAsync);
        }

        [Fact]
        public void GroupByWithPagingOptionsSortDescendingAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGroupByWithPagingOptionsSortDescendingAsync);
        }

        private static void TestDisposeRollBackUnComittedChanges(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();

            uow.Add<Customer>(new Customer { Id = 1 });
            uow.Add<CustomerAddress>(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, uow.Count<Customer>());
            Assert.Equal(1, uow.Count<CustomerAddress>());

            uow.Dispose();

            Assert.Equal(0, uow.Count<Customer>());
            Assert.Equal(0, uow.Count<CustomerAddress>());
        }

        private static void TestComit(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();

            uow.Add<Customer>(new Customer { Id = 1 });
            uow.Add<CustomerAddress>(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, uow.Count<Customer>());
            Assert.Equal(1, uow.Count<CustomerAddress>());

            uow.Commit();
            uow.Dispose();

            Assert.Equal(1, uow.Count<Customer>());
            Assert.Equal(1, uow.Count<CustomerAddress>());
        }

        private static void TestThrowsIfAlreadyComittedOrDisposed(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();

            uow.Dispose();

            var ex = Assert.Throws<InvalidOperationException>(() => uow.Commit());
            Assert.Equal("The transaction has already been committed or disposed.", ex.Message);

            uow = factory.Create();

            uow.Commit();

            ex = Assert.Throws<InvalidOperationException>(() => uow.Commit());
            Assert.Equal("The transaction has already been committed or disposed.", ex.Message);
        }

        private static void TestAdd(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(uow.Exists<Customer>(x => x.Name.Equals(name)));

            uow.Add<Customer>(entity);

            Assert.True(uow.Exists<Customer>(x => x.Name.Equals(name)));
        }

        private static void TestAddRange(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, uow.Count<Customer>());

            uow.Add<Customer>(entities);

            Assert.Equal(2, uow.Count<Customer>());
        }

        private static void TestUpdate(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            uow.Add<Customer>(entity);

            var entityInDb = uow.Find<Customer>(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            uow.Update<Customer>(entityInDb);

            Assert.True(uow.Exists<Customer>(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            uow.Add<Customer>(entities);

            var entitiesInDb = uow.FindAll<Customer>(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            uow.Update<Customer>(entitiesInDb);

            Assert.Equal(2, uow.Count<Customer>(x => x.Name.Equals(expectedName)));
        }

        private static void TestDelete(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            uow.Add<Customer>(entity);

            Assert.True(uow.Exists<Customer>(x => x.Name.Equals(name)));

            var entityInDb = uow.Find<Customer>(x => x.Name.Equals(name));

            uow.Delete<Customer>(entityInDb);

            Assert.False(uow.Exists<Customer>(x => x.Name.Equals(name)));
        }

        private static void TestDeleteRange(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            uow.Add<Customer>(entities);

            Assert.Equal(2, uow.Count<Customer>());

            var entitiesInDb = uow.FindAll<Customer>(x => x.Name.Equals(name));

            uow.Delete<Customer>(entitiesInDb);

            Assert.Equal(0, uow.Count<Customer>());
        }

        private static void TestFind(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Null(uow.Find<Customer>(x => x.Name.Equals(name)));
            Assert.Null(uow.Find<Customer>(options));
            Assert.Null(uow.Find<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(uow.Find<Customer, string>(options, x => x.Name));

            uow.Add<Customer>(entity);

            Assert.NotNull(uow.Find<Customer>(x => x.Name.Equals(name)));
            Assert.NotNull(uow.Find<Customer>(options));
            Assert.Equal(name, uow.Find<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, uow.Find<Customer, string>(options, x => x.Name));
        }

        private static void TestFindWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null(uow.Find<Customer>(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(uow.Find<Customer>(options)?.Name);
            Assert.Null(uow.Find<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(uow.Find<Customer, string>(options, x => x.Name));

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.Find<Customer>(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 2", uow.Find<Customer>(options).Name);
            Assert.Equal("Random Name 2", uow.Find<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", uow.Find<Customer, string>(options, x => x.Name));
        }

        private static void TestFindWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null(uow.Find<Customer>(x => x.Name.Contains("Random Name"))?.Name);
            Assert.Null(uow.Find<Customer>(options)?.Name);
            Assert.Null(uow.Find<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(uow.Find<Customer, string>(options, x => x.Name));

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.Find<Customer>(x => x.Name.Contains("Random Name")).Name);
            Assert.Equal("Random Name 1", uow.Find<Customer>(options).Name);
            Assert.Equal("Random Name 2", uow.Find<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", uow.Find<Customer, string>(options, x => x.Name));
        }

        private static void TestFindAll(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

            Assert.Empty(uow.FindAll<Customer>());
            Assert.Empty(uow.FindAll<Customer>(x => x.Name.Equals(name)));
            Assert.Empty(uow.FindAll<Customer>(options));
            Assert.Empty(uow.FindAll<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(uow.FindAll<Customer, string>(x => x.Name));
            Assert.Empty(uow.FindAll<Customer, string>(options, x => x.Name));

            uow.Add<Customer>(entity);

            Assert.Single(uow.FindAll<Customer>());
            Assert.Single(uow.FindAll<Customer>(x => x.Name.Equals(name)));
            Assert.Single(uow.FindAll<Customer>(options));
            Assert.Single(uow.FindAll<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(uow.FindAll<Customer, string>(x => x.Name));
            Assert.Single(uow.FindAll<Customer, string>(options, x => x.Name));
        }

        private static void TestFindAllWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null(uow.FindAll<Customer>().FirstOrDefault()?.Name);
            Assert.Null(uow.FindAll<Customer>(options).FirstOrDefault()?.Name);
            Assert.Null(uow.FindAll<Customer, string>(x => x.Name).FirstOrDefault());
            Assert.Null(uow.FindAll<Customer, string>(options, x => x.Name).FirstOrDefault());

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.FindAll<Customer>().First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer>(options).First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(x => x.Name).First());
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(options, x => x.Name).First());

            Assert.Equal(1, uow.FindAll<Customer>().First().Id);
            Assert.Equal(1, uow.FindAll<Customer>(options).First().Id);
            Assert.Equal(1, uow.FindAll<Customer, int>(x => x.Id).First());
            Assert.Equal(1, uow.FindAll<Customer, int>(options, x => x.Id).First());

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", uow.FindAll<Customer>().First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer>(options).First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(x => x.Name).First());
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(options, x => x.Name).First());

            Assert.Equal(1, uow.FindAll<Customer>().First().Id);
            Assert.Equal(3, uow.FindAll<Customer>(options).First().Id);
            Assert.Equal(1, uow.FindAll<Customer, int>(x => x.Id).First());
            Assert.Equal(3, uow.FindAll<Customer, int>(options, x => x.Id).First());
        }

        private static void TestFindAllWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null(uow.FindAll<Customer>().FirstOrDefault()?.Name);
            Assert.Null(uow.FindAll<Customer>(options).FirstOrDefault()?.Name);
            Assert.Null(uow.FindAll<Customer, string>(x => x.Name).FirstOrDefault());
            Assert.Null(uow.FindAll<Customer, string>(options, x => x.Name).FirstOrDefault());

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.FindAll<Customer>().First().Name);
            Assert.Equal("Random Name 1", uow.FindAll<Customer>(options).First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(x => x.Name).First());
            Assert.Equal("Random Name 1", uow.FindAll<Customer, string>(options, x => x.Name).First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

            Assert.Equal("Random Name 2", uow.FindAll<Customer>().First().Name);
            Assert.Equal("Random Name 1", uow.FindAll<Customer>(options).First().Name);
            Assert.Equal("Random Name 2", uow.FindAll<Customer, string>(x => x.Name).First());
            Assert.Equal("Random Name 1", uow.FindAll<Customer, string>(options, x => x.Name).First());
        }

        private static void TestFindAllWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            uow.Add<Customer>(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestFindAllWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            uow.Add<Customer>(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = uow.FindAll<Customer>(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static void TestGet(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchStrategy<Customer>();
            fetchStrategy.Include(x => x.Address);

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(uow.Get<Customer>(key));
            Assert.Null(uow.Get<Customer>(key, fetchStrategy));
            Assert.Null(uow.Get<Customer, string>(key, x => x.Name, fetchStrategy));

            uow.Add<Customer>(entity);

            Assert.NotNull(uow.Get<Customer>(key));
            Assert.NotNull(uow.Get<Customer>(key, fetchStrategy));
            Assert.NotNull(uow.Get<Customer, string>(key, x => x.Name, fetchStrategy));

            var ex = Assert.Throws<ArgumentException>(() => uow.Get<Customer>(key.ToString()));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => uow.Get<Customer>(key.ToString(), fetchStrategy));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => uow.Get<Customer, string>(key.ToString(), x => x.Name, fetchStrategy));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);
        }

        private static void TestCount(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, uow.Count<Customer>());
            Assert.Equal(0, uow.Count<Customer>(x => x.Name.Equals(name)));
            Assert.Equal(0, uow.Count<Customer>(options));

            uow.Add<Customer>(entity);

            Assert.Equal(1, uow.Count<Customer>());
            Assert.Equal(1, uow.Count<Customer>(x => x.Name.Equals(name)));
            Assert.Equal(1, uow.Count<Customer>(options));
        }

        private static void TestExists(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(uow.Exists<Customer>(x => x.Name.Equals(name)));
            Assert.False(uow.Exists<Customer>(options));

            uow.Add<Customer>(entity);

            Assert.True(uow.Exists<Customer>(x => x.Name.Equals(name)));
            Assert.True(uow.Exists<Customer>(options));
        }

        private static void TestToDictionary(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            uow.Add<Customer>(entity);

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            uow.Add<Customer>(entities);

            var entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(2);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(3);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(4);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(5);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestToDictionaryWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            uow.Add<Customer>(entities);

            var entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(2);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(3);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(4);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));

            options = options.Page(5);

            entitiesInDb = uow.ToDictionary<Customer, int>(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionary<Customer, int>(options, y => y.Id).ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(y => y.Id, y => y.Name).Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionary<Customer, int, string>(options, y => y.Id, y => y.Name).Contains(x)));
        }

        private static void TestGroupBy(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => uow.GroupBy<Customer, int, int>(y => y.Id, y => y.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => uow.GroupBy<Customer, int, int>(options, y => y.Id, y => y.Key).Contains(x.Key)));

            uow.Add<Customer>(entities);

            Assert.True(expectedGroupByElementSelector.All(x => uow.GroupBy<Customer, int, int>(options, y => y.Id, y => y.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => uow.GroupBy<Customer, int, int>(options, y => y.Id, y => y.Key).Contains(x.Key)));
        }

        private static void TestGroupByWithSortingOptionsAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.GroupBy<Customer, string, string>(options, y => y.Name, x => x.Key).First());
            Assert.Equal("Random Name 2", uow.GroupBy<Customer, string, string>(y => y.Name, x => x.Key).First());
        }

        private static void TestGroupByWithSortingOptionsDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            uow.Add<Customer>(entities);

            Assert.Equal("Random Name 2", uow.GroupBy<Customer, string, string>(y => y.Name, x => x.Key).First());
            Assert.Equal("Random Name 1", uow.GroupBy<Customer, string, string>(options, y => y.Name, x => x.Key).First());
        }

        private static void TestGroupByWithPagingOptionsSortAscending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            uow.Add<Customer>(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
        }

        private static void TestGroupByWithPagingOptionsSortDescending(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            uow.Add<Customer>(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = uow.GroupBy(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestAddAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));

            await uow.AddAsync<Customer>(entity);

            Assert.True(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));
        }

        private static async Task TestAddRangeAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, await uow.CountAsync<Customer>());

            await uow.AddAsync<Customer>(entities);

            Assert.Equal(2, await uow.CountAsync<Customer>());
        }

        private static async Task TestUpdateAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await uow.AddAsync<Customer>(entity);

            var entityInDb = await uow.FindAsync<Customer>(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await uow.UpdateAsync<Customer>(entityInDb);

            Assert.True(await uow.ExistsAsync<Customer>(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await uow.AddAsync<Customer>(entities);

            var entitiesInDb = await uow.FindAllAsync<Customer>(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            await uow.UpdateAsync<Customer>(entitiesInDb);

            Assert.Equal(2, await uow.CountAsync<Customer>(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestDeleteAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await uow.AddAsync<Customer>(entity);

            Assert.True(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));

            var entityInDb = await uow.FindAsync<Customer>(x => x.Name.Equals(name));

            await uow.DeleteAsync<Customer>(entityInDb);

            Assert.False(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteRangeAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            await uow.AddAsync<Customer>(entities);

            Assert.Equal(2, await uow.CountAsync<Customer>());

            var entitiesInDb = await uow.FindAllAsync<Customer>(x => x.Name.Equals(name));

            await uow.DeleteAsync<Customer>(entitiesInDb);

            Assert.Equal(0, await uow.CountAsync<Customer>());
        }

        private static async Task TestFindAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Null(await uow.FindAsync<Customer>(x => x.Name.Equals(name)));
            Assert.Null(await uow.FindAsync<Customer>(options));
            Assert.Null(await uow.FindAsync<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Null(await uow.FindAsync<Customer, string>(options, x => x.Name));

            await uow.AddAsync<Customer>(entity);

            Assert.NotNull(await uow.FindAsync<Customer>(x => x.Name.Equals(name)));
            Assert.NotNull(await uow.FindAsync<Customer>(options));
            Assert.Equal(name, await uow.FindAsync<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Equal(name, await uow.FindAsync<Customer, string>(options, x => x.Name));
        }

        private static async Task TestFindWithSortingOptionsDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null((await uow.FindAsync<Customer>(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await uow.FindAsync<Customer>(options))?.Name);
            Assert.Null(await uow.FindAsync<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await uow.FindAsync<Customer, string>(options, x => x.Name));

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.FindAsync<Customer>(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 2", (await uow.FindAsync<Customer>(options)).Name);
            Assert.Equal("Random Name 2", await uow.FindAsync<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 2", await uow.FindAsync<Customer, string>(options, x => x.Name));
        }

        private static async Task TestFindWithSortingOptionsAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null((await uow.FindAsync<Customer>(x => x.Name.Contains("Random Name")))?.Name);
            Assert.Null((await uow.FindAsync<Customer>(options))?.Name);
            Assert.Null(await uow.FindAsync<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Null(await uow.FindAsync<Customer, string>(options, x => x.Name));

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.FindAsync<Customer>(x => x.Name.Contains("Random Name"))).Name);
            Assert.Equal("Random Name 1", (await uow.FindAsync<Customer>(options)).Name);
            Assert.Equal("Random Name 2", await uow.FindAsync<Customer, string>(x => x.Name.Contains("Random Name"), x => x.Name));
            Assert.Equal("Random Name 1", await uow.FindAsync<Customer, string>(options, x => x.Name));
        }

        private static async Task TestFindAllAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);
            var entity = new Customer { Name = name };

            Assert.Empty(await uow.FindAllAsync<Customer>());
            Assert.Empty(await uow.FindAllAsync<Customer>(x => x.Name.Equals(name)));
            Assert.Empty(await uow.FindAllAsync<Customer>(options));
            Assert.Empty(await uow.FindAllAsync<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Empty(await uow.FindAllAsync<Customer, string>(x => x.Name));
            Assert.Empty(await uow.FindAllAsync<Customer, string>(options, x => x.Name));

            await uow.AddAsync<Customer>(entity);

            Assert.Single(await uow.FindAllAsync<Customer>());
            Assert.Single(await uow.FindAllAsync<Customer>(x => x.Name.Equals(name)));
            Assert.Single(await uow.FindAllAsync<Customer>(options));
            Assert.Single(await uow.FindAllAsync<Customer, string>(x => x.Name.Equals(name), x => x.Name));
            Assert.Single(await uow.FindAllAsync<Customer, string>(x => x.Name));
            Assert.Single(await uow.FindAllAsync<Customer, string>(options, x => x.Name));
        }

        private static async Task TestFindAllWithSortingOptionsDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 2" },
                new Customer { Id = 2, Name = "Random Name 1" },
                new Customer { Id = 3, Name = "Random Name 2" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            Assert.Null((await uow.FindAllAsync<Customer>()).FirstOrDefault()?.Name);
            Assert.Null((await uow.FindAllAsync<Customer>(options)).FirstOrDefault()?.Name);
            Assert.Null((await uow.FindAllAsync<Customer, string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await uow.FindAllAsync<Customer, string>(options, x => x.Name)).FirstOrDefault());

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>()).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>(options)).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(options, x => x.Name)).First());

            Assert.Equal(1, (await uow.FindAllAsync<Customer>()).First().Id);
            Assert.Equal(1, (await uow.FindAllAsync<Customer>(options)).First().Id);
            Assert.Equal(1, (await uow.FindAllAsync<Customer, int>(x => x.Id)).First());
            Assert.Equal(1, (await uow.FindAllAsync<Customer, int>(options, x => x.Id)).First());

            options = new QueryOptions<Customer>().SortByDescending(x => x.Name).SortByDescending(x => x.Id);

            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>()).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>(options)).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(x => x.Name)).First());
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(options, x => x.Name)).First());

            Assert.Equal(1, (await uow.FindAllAsync<Customer>()).First().Id);
            Assert.Equal(3, (await uow.FindAllAsync<Customer>(options)).First().Id);
            Assert.Equal(1, (await uow.FindAllAsync<Customer, int>(x => x.Id)).First());
            Assert.Equal(3, (await uow.FindAllAsync<Customer, int>(options, x => x.Id)).First());
        }

        private static async Task TestFindAllWithSortingOptionsAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            Assert.Null((await uow.FindAllAsync<Customer>()).FirstOrDefault()?.Name);
            Assert.Null((await uow.FindAllAsync<Customer>(options)).FirstOrDefault()?.Name);
            Assert.Null((await uow.FindAllAsync<Customer, string>(x => x.Name)).FirstOrDefault());
            Assert.Null((await uow.FindAllAsync<Customer, string>(options, x => x.Name)).FirstOrDefault());

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>()).First().Name);
            Assert.Equal("Random Name 1", (await uow.FindAllAsync<Customer>(options)).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await uow.FindAllAsync<Customer, string>(options, x => x.Name)).First());

            options = new QueryOptions<Customer>().SortBy(x => x.Name).SortBy(x => x.Id);

            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer>()).First().Name);
            Assert.Equal("Random Name 1", (await uow.FindAllAsync<Customer>(options)).First().Name);
            Assert.Equal("Random Name 2", (await uow.FindAllAsync<Customer, string>(x => x.Name)).First());
            Assert.Equal("Random Name 1", (await uow.FindAllAsync<Customer, string>(options, x => x.Name)).First());
        }

        private static async Task TestFindAllWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await uow.AddAsync<Customer>(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestFindAllWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await uow.AddAsync<Customer>(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Name);

            options = options.Page(2);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Name);

            options = options.Page(3);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Name);

            options = options.Page(4);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Name);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Name);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Name);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Name);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Name);

            options = options.Page(5);

            entitiesInDb = await uow.FindAllAsync<Customer>(options);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Name);
        }

        private static async Task TestGetAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            int key = 1;
            const string name = "Random Name";

            var fetchStrategy = new FetchStrategy<Customer>();
            fetchStrategy.Include(x => x.Address);

            var entity = new Customer { Id = key, Name = name };

            Assert.Null(await uow.GetAsync<Customer>(key));
            Assert.Null(await uow.GetAsync<Customer>(key, fetchStrategy));
            Assert.Null(await uow.GetAsync<Customer, string>(key, x => x.Name, fetchStrategy));

            uow.Add<Customer>(entity);

            Assert.NotNull(await uow.GetAsync<Customer>(key));
            Assert.NotNull(await uow.GetAsync<Customer>(key, fetchStrategy));
            Assert.NotNull(await uow.GetAsync<Customer, string>(key, x => x.Name, fetchStrategy));

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => uow.GetAsync<Customer>(key.ToString()));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => uow.GetAsync<Customer>(key.ToString(), fetchStrategy));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => uow.GetAsync<Customer, string>(key.ToString(), x => x.Name, fetchStrategy));
            Assert.Equal($"The key value was of type '{typeof(string)}', which does not match the property type of '{typeof(int)}'.", ex.Message);
        }

        private static async Task TestCountAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.Equal(0, await uow.CountAsync<Customer>());
            Assert.Equal(0, await uow.CountAsync<Customer>(x => x.Name.Equals(name)));
            Assert.Equal(0, await uow.CountAsync<Customer>(options));

            await uow.AddAsync<Customer>(entity);

            Assert.Equal(1, await uow.CountAsync<Customer>());
            Assert.Equal(1, await uow.CountAsync<Customer>(x => x.Name.Equals(name)));
            Assert.Equal(1, await uow.CountAsync<Customer>(options));
        }

        private static async Task TestExistsAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Name = name };

            Assert.False(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));
            Assert.False(await uow.ExistsAsync<Customer>(options));

            await uow.AddAsync<Customer>(entity);

            Assert.True(await uow.ExistsAsync<Customer>(x => x.Name.Equals(name)));
            Assert.True(await uow.ExistsAsync<Customer>(options));
        }

        private static async Task TestToDictionaryAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };
            var expectedDictionary = new Dictionary<int, Customer>();
            var expectedDictionaryByElementSelector = new Dictionary<int, string>();

            expectedDictionary.Add(entity.Id, entity);
            expectedDictionaryByElementSelector.Add(entity.Id, entity.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await uow.AddAsync<Customer>(entity);

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await uow.AddAsync<Customer>(entities);

            var entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[20].Id, entities[20]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[20].Id, entities[20].Name);

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestToDictionaryWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i + 1, Name = "Random Name " + i });
            }

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);

            var expectedDictionary = entities.ToDictionary(x => x.Id);
            var expectedDictionaryByElementSelector = entities.ToDictionary(x => x.Id, y => y.Name);

            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            await uow.AddAsync<Customer>(entities);

            var entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(2);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(3);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(4);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

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

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.False(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.False(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));

            options = options.Page(5);

            entitiesInDb = await uow.ToDictionaryAsync<Customer, int>(options, x => x.Id);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Value.Name);

            expectedDictionary.Clear();
            expectedDictionary.Add(entities[0].Id, entities[0]);

            expectedDictionaryByElementSelector.Clear();
            expectedDictionaryByElementSelector.Add(entities[0].Id, entities[0].Name);

            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionary.All(x => uow.ToDictionaryAsync<Customer, int>(options, y => y.Id).Result.ContainsKey(x.Key)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(y => y.Id, y => y.Name).Result.Contains(x)));
            Assert.True(expectedDictionaryByElementSelector.All(x => uow.ToDictionaryAsync<Customer, int, string>(options, y => y.Id, y => y.Name).Result.Contains(x)));
        }

        private static async Task TestGroupByAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => uow.GroupByAsync<Customer, int, int>(y => y.Id, y => y.Key).Result.Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => uow.GroupByAsync<Customer, int, int>(options, y => y.Id, y => y.Key).Result.Contains(x.Key)));

            await uow.AddAsync<Customer>(entities);

            Assert.True(expectedGroupByElementSelector.All(x => uow.GroupByAsync<Customer, int, int>(y => y.Id, y => y.Key).Result.Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => uow.GroupByAsync<Customer, int, int>(options, y => y.Id, y => y.Key).Result.Contains(x.Key)));
        }

        private static async Task TestGroupByWithSortingOptionsAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Name);

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.GroupByAsync<Customer, string, string>(options, y => y.Name, x => x.Key)).First());
            Assert.Equal("Random Name 2", (await uow.GroupByAsync<Customer, string, string>(y => y.Name, x => x.Key)).First());
        }

        private static async Task TestGroupByWithSortingOptionsDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>
            {
                new Customer { Name = "Random Name 2" },
                new Customer { Name = "Random Name 1" }
            };

            var options = new QueryOptions<Customer>().SortBy(x => x.Name);

            await uow.AddAsync<Customer>(entities);

            Assert.Equal("Random Name 2", (await uow.GroupByAsync<Customer, string, string>(y => y.Name, x => x.Key)).First());
            Assert.Equal("Random Name 1", (await uow.GroupByAsync<Customer, string, string>(options, y => y.Name, x => x.Key)).First());
        }

        private static async Task TestGroupByWithPagingOptionsSortAscendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await uow.AddAsync<Customer>(entities);

            var options = new QueryOptions<Customer>().SortBy(x => x.Id).Page(1, 5);
            var entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
        }

        private static async Task TestGroupByWithPagingOptionsSortDescendingAsync(IUnitOfWorkFactoryAsync uowFactory)
        {
            var uow = uowFactory.CreateAsync();

            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            await uow.AddAsync<Customer>(entities);

            var options = new QueryOptions<Customer>().SortByDescending(x => x.Id).Page(1, 5);
            var entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 20", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 19", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 18", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 17", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 16", entitiesInDb.ElementAt(4).Key);

            options = options.Page(2);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 15", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 14", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 13", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 12", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 11", entitiesInDb.ElementAt(4).Key);

            options = options.Page(3);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 10", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 9", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 8", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 7", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 6", entitiesInDb.ElementAt(4).Key);

            options = options.Page(4);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Equal(5, entitiesInDb.Count());
            Assert.Equal("Random Name 5", entitiesInDb.ElementAt(0).Key);
            Assert.Equal("Random Name 4", entitiesInDb.ElementAt(1).Key);
            Assert.Equal("Random Name 3", entitiesInDb.ElementAt(2).Key);
            Assert.Equal("Random Name 2", entitiesInDb.ElementAt(3).Key);
            Assert.Equal("Random Name 1", entitiesInDb.ElementAt(4).Key);

            options = options.Page(5);

            entitiesInDb = await uow.GroupByAsync(options, y => y.Name, y => y);

            Assert.Single(entitiesInDb);
            Assert.Equal("Random Name 0", entitiesInDb.ElementAt(0).Key);
        }
    }
}
