namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using Data;
    using Internal;
    using Moq;
    using Query;
    using Query.Strategies;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryReadOnlyTests : TestBase
    {
        public RepositoryReadOnlyTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Find()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.Find(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.Find(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.Find(It.IsAny<IQueryOptions<Customer>>()));
            mock.Setup(x => x.Find(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.Find(predicate);
            readOnlyRepo.Find(predicate, selector);
            readOnlyRepo.Find(options);
            readOnlyRepo.Find(options, selector);

            mock.Verify(x => x.Find(predicate), Times.Once);
            mock.Verify(x => x.Find(predicate, selector), Times.Once);
            mock.Verify(x => x.Find(options), Times.Once);
            mock.Verify(x => x.Find(options, selector), Times.Once);
        }

        [Fact]
        public void FindWithId()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var fetchStrategy = new FetchQueryStrategy<Customer>();
            Expression<Func<Customer, string>> selector = x => x.Name;

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.Find(It.IsAny<int>()));
            mock.Setup(x => x.Find(It.IsAny<int>(), It.IsAny<IFetchQueryStrategy<Customer>>()));
            mock.Setup(x => x.Find(It.IsAny<int>(), It.IsAny<string[]>()));
            mock.Setup(x => x.Find(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.Find(key);
            readOnlyRepo.Find(key, fetchStrategy);
            readOnlyRepo.Find(key, string.Empty);
            readOnlyRepo.Find(key, (Expression<Func<Customer, object>>[])null);

            mock.Verify(x => x.Find(key), Times.Once);
            mock.Verify(x => x.Find(key, fetchStrategy), Times.Once);
            mock.Verify(x => x.Find(key, string.Empty), Times.Once);
            mock.Verify(x => x.Find(key, (Expression<Func<Customer, object>>[])null), Times.Once);
        }

        [Fact]
        public void FindAll()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.FindAll());
            mock.Setup(x => x.FindAll(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.FindAll(It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.FindAll(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.FindAll(It.IsAny<IQueryOptions<Customer>>()));
            mock.Setup(x => x.FindAll(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.FindAll();
            readOnlyRepo.FindAll(selector);
            readOnlyRepo.FindAll(predicate);
            readOnlyRepo.FindAll(predicate, selector);
            readOnlyRepo.FindAll(options);
            readOnlyRepo.FindAll(options, selector);

            mock.Verify(x => x.FindAll(), Times.Once);
            mock.Verify(x => x.FindAll(selector), Times.Once);
            mock.Verify(x => x.FindAll(predicate), Times.Once);
            mock.Verify(x => x.FindAll(predicate, selector), Times.Once);
            mock.Verify(x => x.FindAll(options), Times.Once);
            mock.Verify(x => x.FindAll(options, selector), Times.Once);
        }

        [Fact]
        public void Exists()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.Exists(It.IsAny<int>()));
            mock.Setup(x => x.Exists(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.Exists(It.IsAny<IQueryOptions<Customer>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.Exists(key);
            readOnlyRepo.Exists(predicate);
            readOnlyRepo.Exists(options);

            mock.Verify(x => x.Exists(key), Times.Once);
            mock.Verify(x => x.Exists(predicate), Times.Once);
            mock.Verify(x => x.Exists(options), Times.Once);
        }

        [Fact]
        public void Count()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.Count());
            mock.Setup(x => x.Count(It.IsAny<Expression<Func<Customer, bool>>>()));
            mock.Setup(x => x.Count(It.IsAny<IQueryOptions<Customer>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.Count();
            readOnlyRepo.Count(predicate);
            readOnlyRepo.Count(options);

            mock.Verify(x => x.Count(), Times.Once);
            mock.Verify(x => x.Count(predicate), Times.Once);
            mock.Verify(x => x.Count(options), Times.Once);
        }

        [Fact]
        public void ToDictionary()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.ToDictionary(It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.ToDictionary(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.ToDictionary(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>()));
            mock.Setup(x => x.ToDictionary(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.ToDictionary(selector);
            readOnlyRepo.ToDictionary(selector, selector);
            readOnlyRepo.ToDictionary(options, selector);
            readOnlyRepo.ToDictionary(options, selector, selector);

            mock.Verify(x => x.ToDictionary(selector), Times.Once);
            mock.Verify(x => x.ToDictionary(selector, selector), Times.Once);
            mock.Verify(x => x.ToDictionary(options, selector), Times.Once);
            mock.Verify(x => x.ToDictionary(options, selector, selector), Times.Once);
        }

        [Fact]
        public void GroupBy()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            Expression<Func<IGrouping<string, Customer>, string>> grouping = z => z.Key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.GroupBy(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>()));
            mock.Setup(x => x.GroupBy(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            readOnlyRepo.GroupBy(selector, grouping);
            readOnlyRepo.GroupBy(options, selector, grouping);

            mock.Verify(x => x.GroupBy(selector, grouping), Times.Once);
            mock.Verify(x => x.GroupBy(options, selector, grouping), Times.Once);
        }

        [Fact]
        public async Task FindAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.FindAsync(predicate);
            await readOnlyRepo.FindAsync(predicate, selector);
            await readOnlyRepo.FindAsync(options);
            await readOnlyRepo.FindAsync(options, selector);

            mock.Verify(x => x.FindAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(predicate, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(options, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(options, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task FindWithIdAsync()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var fetchStrategy = new FetchQueryStrategy<Customer>();
            Expression<Func<Customer, string>> selector = x => x.Name;

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<IFetchQueryStrategy<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<string[]>()));
            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAsync(It.IsAny<int>(), It.IsAny<Expression<Func<Customer, object>>[]>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.FindAsync(key);
            await readOnlyRepo.FindAsync(key, fetchStrategy);
            await readOnlyRepo.FindAsync(key, new[] { string.Empty }, default(CancellationToken));
            await readOnlyRepo.FindAsync(key, string.Empty);
            await readOnlyRepo.FindAsync(key, (Expression<Func<Customer, object>>[])null);
            await readOnlyRepo.FindAsync(key, (Expression<Func<Customer, object>>[])null, default(CancellationToken));

            mock.Verify(x => x.FindAsync(key, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(key, fetchStrategy, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(key, new[] { string.Empty }, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAsync(key, string.Empty), Times.Once);
            mock.Verify(x => x.FindAsync(key, (Expression<Func<Customer, object>>[])null), Times.Once);
            mock.Verify(x => x.FindAsync(key, (Expression<Func<Customer, object>>[])null, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task FindAllAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.FindAllAsync(It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAllAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.FindAllAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.FindAllAsync();
            await readOnlyRepo.FindAllAsync(selector);
            await readOnlyRepo.FindAllAsync(predicate);
            await readOnlyRepo.FindAllAsync(predicate, selector);
            await readOnlyRepo.FindAllAsync(options);
            await readOnlyRepo.FindAllAsync(options, selector);

            mock.Verify(x => x.FindAllAsync(default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAllAsync(selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAllAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAllAsync(predicate, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAllAsync(options, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.FindAllAsync(options, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.ExistsAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.ExistsAsync(key);
            await readOnlyRepo.ExistsAsync(predicate);
            await readOnlyRepo.ExistsAsync(options);

            mock.Verify(x => x.ExistsAsync(key, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.ExistsAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.ExistsAsync(options, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task CountAsync()
        {
            Expression<Func<Customer, bool>> predicate = x => true;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.CountAsync(It.IsAny<CancellationToken>()));
            mock.Setup(x => x.CountAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.CountAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.CountAsync();
            await readOnlyRepo.CountAsync(predicate);
            await readOnlyRepo.CountAsync(options);

            mock.Verify(x => x.CountAsync(default(CancellationToken)), Times.Once);
            mock.Verify(x => x.CountAsync(predicate, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.CountAsync(options, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task ToDictionaryAsync()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.ToDictionaryAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.ToDictionaryAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.ToDictionaryAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.ToDictionaryAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.ToDictionaryAsync(selector);
            await readOnlyRepo.ToDictionaryAsync(selector, selector);
            await readOnlyRepo.ToDictionaryAsync(options, selector);
            await readOnlyRepo.ToDictionaryAsync(options, selector, selector);

            mock.Verify(x => x.ToDictionaryAsync(selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.ToDictionaryAsync(selector, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.ToDictionaryAsync(options, selector, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.ToDictionaryAsync(options, selector, selector, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task GroupByAsync()
        {
            Expression<Func<Customer, string>> selector = x => x.Name;
            Expression<Func<IGrouping<string, Customer>, string>> grouping = z => z.Key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.GroupByAsync(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>(), It.IsAny<CancellationToken>()));
            mock.Setup(x => x.GroupByAsync(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<IGrouping<string, Customer>, string>>>(), It.IsAny<CancellationToken>()));

            var readOnlyRepo = new ReadOnlyRepositoryWrapper<Customer, int>(mock.Object);

            await readOnlyRepo.GroupByAsync(selector, grouping);
            await readOnlyRepo.GroupByAsync(options, selector, grouping);

            mock.Verify(x => x.GroupByAsync(selector, grouping, default(CancellationToken)), Times.Once);
            mock.Verify(x => x.GroupByAsync(options, selector, grouping, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public void AsReadOnly()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            Assert.True(repo.AsReadOnly() is ReadOnlyRepositoryWrapper<Customer, int>);
        }
    }
}
