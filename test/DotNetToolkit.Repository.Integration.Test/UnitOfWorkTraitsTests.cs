namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using System;
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

        private static void TestDisposeRollBackUnComittedChanges(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();

            uow.Create<Customer>().Add(new Customer { Id = 1 });
            uow.Create<CustomerAddress>().Add(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, uow.Create<Customer>().Count());
            Assert.Equal(1, uow.Create<CustomerAddress>().Count());

            uow.Dispose();

            Assert.Equal(0, uow.Create<Customer>().Count());
            Assert.Equal(0, uow.Create<CustomerAddress>().Count());
        }

        private static void TestComit(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();

            uow.Create<Customer>().Add(new Customer { Id = 1 });
            uow.Create<CustomerAddress>().Add(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, uow.Create<Customer>().Count());
            Assert.Equal(1, uow.Create<CustomerAddress>().Count());

            uow.Commit();
            uow.Dispose();

            Assert.Equal(1, uow.Create<Customer>().Count());
            Assert.Equal(1, uow.Create<CustomerAddress>().Count());
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
    }
}
