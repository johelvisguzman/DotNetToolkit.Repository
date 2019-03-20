namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using System;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class UnitOfWorkTests : TestBase
    {
        public UnitOfWorkTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void FactoryCreate()
        {
            ForAllUnitOfWorkFactories(TestFactoryCreate);
        }
        
        [Fact]
        public void DisposeRollBackUnCommittedChanges()
        {
            ForAllUnitOfWorkFactories(TestDisposeRollBackUnCommittedChanges);
        }

        [Fact]
        public void Commit()
        {
            ForAllUnitOfWorkFactories(TestCommit);
        }

        [Fact]
        public void ThrowsIfAlreadyCommitted()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfAlreadyCommitted);
        }

        [Fact]
        public void ThrowsIfCommitWhenDisposed()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfCommitWhenDisposed);
        }

        [Fact]
        public void Create()
        {
            ForAllUnitOfWorkFactories(TestCreate);
        }

        [Fact]
        public void ThrowsIfCreateRepositoryWhenDisposed()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfCreateRepositoryWhenDisposed);
        }

        private static void TestFactoryCreate(IUnitOfWorkFactory uowFactory)
        {
            Assert.NotNull(uowFactory.Create());
            Assert.NotNull(uowFactory.CreateInstance<UnitOfWork>());
        }

        private static void TestDisposeRollBackUnCommittedChanges(IUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                uow.Create<Customer>().Add(new Customer { Id = 1 });
                uow.Create<CustomerAddress>().Add(new CustomerAddress() { CustomerId = 1 });

                Assert.Equal(1, uow.Create<Customer>().Count());
                Assert.Equal(1, uow.Create<CustomerAddress>().Count());
            }

            using (var uow = uowFactory.Create())
            {
                Assert.Equal(0, uow.Create<Customer>().Count());
                Assert.Equal(0, uow.Create<CustomerAddress>().Count());
            }
        }

        private static void TestCommit(IUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                uow.Create<Customer>().Add(new Customer { Id = 1 });
                uow.Create<CustomerAddress>().Add(new CustomerAddress() { CustomerId = 1 });

                Assert.Equal(1, uow.Create<Customer>().Count());
                Assert.Equal(1, uow.Create<CustomerAddress>().Count());

                uow.Commit();
            }

            using (var uow = uowFactory.Create())
            {
                Assert.Equal(1, uow.Create<Customer>().Count());
                Assert.Equal(1, uow.Create<CustomerAddress>().Count());
            }
        }

        private static void TestThrowsIfAlreadyCommitted(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            uow.Commit();

            var ex = Assert.Throws<InvalidOperationException>(() => uow.Commit());
            Assert.Equal("The transaction has already been committed.", ex.Message);
        }

        private static void TestThrowsIfCommitWhenDisposed(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            uow.Dispose();

            var ex = Assert.Throws<ObjectDisposedException>(() => uow.Commit());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);
        }

        private static void TestCreate(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            Assert.NotNull(uow.Create<Customer>());
            Assert.NotNull(uow.Create<Customer, int>());
            Assert.NotNull(uow.Create<CustomerWithTwoCompositePrimaryKey, int, string>());
            Assert.NotNull(uow.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>());
            Assert.NotNull(uow.CreateInstance<Repository<Customer>>());
        }

        private static void TestThrowsIfCreateRepositoryWhenDisposed(IUnitOfWorkFactory uowFactory)
        {
            var uow = uowFactory.Create();

            uow.Dispose();

            var ex = Assert.Throws<ObjectDisposedException>(() => uow.Create<Customer>());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);

            ex = Assert.Throws<ObjectDisposedException>(() => uow.Create<Customer, int>());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);

            ex = Assert.Throws<ObjectDisposedException>(() => uow.Create<Customer, int, int>());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);

            ex = Assert.Throws<ObjectDisposedException>(() => uow.Create<Customer, int, int, int>());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);

            ex = Assert.Throws<ObjectDisposedException>(() => uow.CreateInstance<Repository<Customer>>());
            Assert.Equal("Cannot access a disposed object.\r\nObject name: 'UnitOfWork'.", ex.Message);
        }
    }
}
