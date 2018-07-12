namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using FetchStrategies;
    using Moq;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Wrappers;
    using Xunit;

    public class ReadOnlyRepositoryTraitsTests : TestBase
    {
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

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

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

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

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

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

            readOnlyRepo.Exists(key);
            readOnlyRepo.Exists(predicate);
            readOnlyRepo.Exists(options);

            mock.Verify(x => x.Exists(key), Times.Once);
            mock.Verify(x => x.Exists(predicate), Times.Once);
            mock.Verify(x => x.Exists(options), Times.Once);
        }

        [Fact]
        public void Get()
        {
            int key = 1;
            Expression<Func<Customer, bool>> predicate = x => true;
            var fetchStrategy = new FetchStrategy<Customer>();
            Expression<Func<Customer, string>> selector = x => x.Name;

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.Get(It.IsAny<int>()));
            mock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<IFetchStrategy<Customer>>()));

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

            readOnlyRepo.Get(key);
            readOnlyRepo.Get(key, fetchStrategy);

            mock.Verify(x => x.Get(key), Times.Once);
            mock.Verify(x => x.Get(key, fetchStrategy), Times.Once);
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

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

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

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

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
            Expression<Func<string, IEnumerable<Customer>, string>> grouping = (key, g) => key;
            var options = new QueryOptions<Customer>();

            var mock = new Mock<IRepository<Customer>>();

            mock.Setup(x => x.GroupBy(It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<string, IEnumerable<Customer>, string>>>()));
            mock.Setup(x => x.GroupBy(It.IsAny<IQueryOptions<Customer>>(), It.IsAny<Expression<Func<Customer, string>>>(), It.IsAny<Expression<Func<string, IEnumerable<Customer>, string>>>()));

            var readOnlyRepo = new ReadOnlyRepository<Customer, int>(mock.Object);

            readOnlyRepo.GroupBy(selector, grouping);
            readOnlyRepo.GroupBy(options, selector, grouping);

            mock.Verify(x => x.GroupBy(selector, grouping), Times.Once);
            mock.Verify(x => x.GroupBy(options, selector, grouping), Times.Once);
        }

        [Fact]
        public void AsReadOnly()
        {
            ForAllRepositoryFactories(TestAsReadOnly);
        }

        private static void TestAsReadOnly(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var readOnlyRepo = repo.AsReadOnly();

            Assert.Equal(readOnlyRepo, repo.AsReadOnly());
        }
    }
}
