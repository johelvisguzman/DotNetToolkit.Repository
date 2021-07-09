namespace DotNetToolkit.Repository.Integration.Test.Service
{
    using Data;
    using Internal;
    using Moq;
    using Query;
    using Query.Strategies;
    using Services;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class ServiceReadOnlyTests : TestBase
    {
        public ServiceReadOnlyTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Get()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.Get(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.Get(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.Get(It.IsAny<IQueryOptions<Customer>>()));
            mock.Setup(x => x.Get(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.Get(predicate);
            readOnlyService.Get(predicate, selector);
            readOnlyService.Get(options);
            readOnlyService.Get(options, selector);

            mock.Verify(x => x.Get(predicate), Times.Once);
            mock.Verify(x => x.Get(predicate, selector), Times.Once);
            mock.Verify(x => x.Get(options), Times.Once);
            mock.Verify(x => x.Get(options, selector), Times.Once);
        }

        [Fact]
        public void GetWithId()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var fetchStrategy = new FetchQueryStrategy<Customer>();
            Expression<Func<Customer, string>> selector = x => x.Name;

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.Get(It.IsAny<int>()));
            mock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<IFetchQueryStrategy<Customer>>()));
            mock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<string[]>()));
            mock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.Get(key);
            readOnlyService.Get(key, fetchStrategy);
            readOnlyService.Get(key, string.Empty);
            readOnlyService.Get(key, (Expression<Func<Customer, object>>[])null);

            mock.Verify(x => x.Get(key), Times.Once);
            mock.Verify(x => x.Get(key, fetchStrategy), Times.Once);
            mock.Verify(x => x.Get(key, string.Empty), Times.Once);
            mock.Verify(x => x.Get(key, (Expression<Func<Customer, object>>[])null), Times.Once);
        }

        [Fact]
        public void GetAll()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetAll());
            mock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.GetAll(It.IsAny<IQueryOptions<Customer>>()));
            mock.Setup(x => x.GetAll(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetAll();
            readOnlyService.GetAll(selector);
            readOnlyService.GetAll(predicate);
            readOnlyService.GetAll(predicate, selector);
            readOnlyService.GetAll(options);
            readOnlyService.GetAll(options, selector);

            mock.Verify(x => x.GetAll(), Times.Once);
            mock.Verify(x => x.GetAll(selector), Times.Once);
            mock.Verify(x => x.GetAll(predicate), Times.Once);
            mock.Verify(x => x.GetAll(predicate, selector), Times.Once);
            mock.Verify(x => x.GetAll(options), Times.Once);
            mock.Verify(x => x.GetAll(options, selector), Times.Once);
        }

        [Fact]
        public void GetExists()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetExists(It.IsAny<int>()));
            mock.Setup(x => x.GetExists(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.GetExists(It.IsAny<IQueryOptions<Customer>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetExists(key);
            readOnlyService.GetExists(predicate);
            readOnlyService.GetExists(options);

            mock.Verify(x => x.GetExists(key), Times.Once);
            mock.Verify(x => x.GetExists(predicate), Times.Once);
            mock.Verify(x => x.GetExists(options), Times.Once);
        }

        [Fact]
        public void GetCount()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetCount());
            mock.Setup(x => x.GetCount(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.GetCount(It.IsAny<IQueryOptions<Customer>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetCount();
            readOnlyService.GetCount(predicate);
            readOnlyService.GetCount(options);

            mock.Verify(x => x.GetCount(), Times.Once);
            mock.Verify(x => x.GetCount(predicate), Times.Once);
            mock.Verify(x => x.GetCount(options), Times.Once);
        }

        [Fact]
        public void GetDictionary()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetDictionary(It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.GetDictionary(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.GetDictionary(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.GetDictionary(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetDictionary(selector);
            readOnlyService.GetDictionary(selector, selector);
            readOnlyService.GetDictionary(options, selector);
            readOnlyService.GetDictionary(options, selector, selector);

            mock.Verify(x => x.GetDictionary(selector), Times.Once);
            mock.Verify(x => x.GetDictionary(selector, selector), Times.Once);
            mock.Verify(x => x.GetDictionary(options, selector), Times.Once);
            mock.Verify(x => x.GetDictionary(options, selector, selector), Times.Once);
        }

        [Fact]
        public void GroupBy()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            Expression<Func<IGrouping<string, Customer>, string>> grouping = z => z.Key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetGroupBy(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>()));
            mock.Setup(x => x.GetGroupBy(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetGroupBy(selector, grouping);
            readOnlyService.GetGroupBy(options, selector, grouping);

            mock.Verify(x => x.GetGroupBy(selector, grouping), Times.Once);
            mock.Verify(x => x.GetGroupBy(options, selector, grouping), Times.Once);
        }

        [Fact]
        public async Task GetAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetAsync(predicate);
            await readOnlyService.GetAsync(predicate, selector);
            await readOnlyService.GetAsync(options);
            await readOnlyService.GetAsync(options, selector);

            mock.Verify(x => x.GetAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(predicate, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(options, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(options, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GetWithIdAsync()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var fetchStrategy = new FetchQueryStrategy<Customer>();
            Expression<Func<Customer, string>> selector = x => x.Name;

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IFetchQueryStrategy<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string[]>()));
            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetAsync(key);
            await readOnlyService.GetAsync(key, fetchStrategy);
            await readOnlyService.GetAsync(key, new[] { string.Empty }, default(CancellationToken));
            await readOnlyService.GetAsync(key, string.Empty);
            await readOnlyService.GetAsync(key, (Expression<Func<Customer, object>>[])null);
            await readOnlyService.GetAsync(key, (Expression<Func<Customer, object>>[])null, default(CancellationToken));

            mock.Verify(x => x.GetAsync(key, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(key, fetchStrategy, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(key, new[] { string.Empty }, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAsync(key, string.Empty), Times.Once);
            mock.Verify(x => x.GetAsync(key, (Expression<Func<Customer, object>>[])null), Times.Once);
            mock.Verify(x => x.GetAsync(key, (Expression<Func<Customer, object>>[])null, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAllAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetAllAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetAllAsync();
            await readOnlyService.GetAllAsync(selector);
            await readOnlyService.GetAllAsync(predicate);
            await readOnlyService.GetAllAsync(predicate, selector);
            await readOnlyService.GetAllAsync(options);
            await readOnlyService.GetAllAsync(options, selector);

            mock.Verify(x => x.GetAllAsync(default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAllAsync(selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAllAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAllAsync(predicate, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAllAsync(options, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetAllAsync(options, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GetExistsAsync()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetExistsAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetExistsAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetExistsAsync(key);
            await readOnlyService.GetExistsAsync(predicate);
            await readOnlyService.GetExistsAsync(options);

            mock.Verify(x => x.GetExistsAsync(key, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetExistsAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetExistsAsync(options, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GetCountAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetCountAsync(default(CancellationToken)));
            mock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetCountAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetCountAsync();
            await readOnlyService.GetCountAsync(predicate);
            await readOnlyService.GetCountAsync(options);

            mock.Verify(x => x.GetCountAsync(default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetCountAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetCountAsync(options, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GetDictionaryAsync()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetDictionaryAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetDictionaryAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetDictionaryAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetDictionaryAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetDictionaryAsync(selector);
            await readOnlyService.GetDictionaryAsync(selector, selector);
            await readOnlyService.GetDictionaryAsync(options, selector);
            await readOnlyService.GetDictionaryAsync(options, selector, selector);

            mock.Verify(x => x.GetDictionaryAsync(selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetDictionaryAsync(selector, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetDictionaryAsync(options, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetDictionaryAsync(options, selector, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GroupByAsync()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            Expression<Func<IGrouping<string, Customer>, string>> grouping = z => z.Key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetGroupByAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GetGroupByAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            await readOnlyService.GetGroupByAsync(selector, grouping);
            await readOnlyService.GetGroupByAsync(options, selector, grouping);

            mock.Verify(x => x.GetGroupByAsync(selector, grouping, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GetGroupByAsync(options, selector, grouping, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public void AsReadOnly()
        {
            var service = new Service<Customer>(new UnitOfWorkFactory(BuildOptions(ContextProviderType.InMemory)));

            Assert.True(service.AsReadOnly() is ReadOnlyServiceWrapper<Customer, int>);
        }
    }
}
