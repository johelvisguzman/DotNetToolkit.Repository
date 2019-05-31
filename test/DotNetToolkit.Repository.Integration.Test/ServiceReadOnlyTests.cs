namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Internal;
    using Moq;
    using Queries;
    using Queries.Strategies;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
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

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.Get(key);
            readOnlyService.Get(key, fetchStrategy);

            mock.Verify(x => x.Get(key), Times.Once);
            mock.Verify(x => x.Get(key, fetchStrategy), Times.Once);
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
            Expression<Func<string, IEnumerable<Customer>, string>> grouping = (key, g) => key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IService<Customer>>();

            mock.Setup(x => x.GetGroupBy(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<string, IEnumerable<Customer>, string>>>()));
            mock.Setup(x => x.GetGroupBy(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<string, IEnumerable<Customer>, string>>>()));

            var readOnlyService = new ReadOnlyServiceWrapper<Customer, int>(mock.Object);

            readOnlyService.GetGroupBy(selector, grouping);
            readOnlyService.GetGroupBy(options, selector, grouping);

            mock.Verify(x => x.GetGroupBy(selector, grouping), Times.Once);
            mock.Verify(x => x.GetGroupBy(options, selector, grouping), Times.Once);
        }

        [Fact]
        public void AsReadOnly()
        {
            var service = new Service<Customer>(new UnitOfWorkFactory(BuildOptions(ContextProviderType.InMemory)));

            Assert.True(service.AsReadOnly() is ReadOnlyServiceWrapper<Customer, int>);
        }
    }
}
