namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using System;
    using Xunit;

    public class UnitOfWorkTests : TestBase
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

        private void TestDisposeRollBackUnComittedChanges(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();
            var customerRepo = uow.GetRepository<Customer>();
            var customerAddressRepo = uow.GetRepository<CustomerAddress>();

            customerRepo.Add(new Customer { Id = 1 });
            customerAddressRepo.Add(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, customerRepo.Count());
            Assert.Equal(1, customerAddressRepo.Count());

            uow.Dispose();

            Assert.Equal(0, customerRepo.Count());
            Assert.Equal(0, customerAddressRepo.Count());
        }

        private void TestComit(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();
            var customerRepo = uow.GetRepository<Customer>();
            var customerAddressRepo = uow.GetRepository<CustomerAddress>();

            customerRepo.Add(new Customer { Id = 1 });
            customerAddressRepo.Add(new CustomerAddress() { CustomerId = 1 });

            Assert.Equal(1, customerRepo.Count());
            Assert.Equal(1, customerAddressRepo.Count());

            uow.Commit();
            uow.Dispose();

            Assert.Equal(1, customerRepo.Count());
            Assert.Equal(1, customerAddressRepo.Count());
        }

        private void TestThrowsIfAlreadyComittedOrDisposed(IUnitOfWorkFactory factory)
        {
            var uow = factory.Create();
            uow.GetRepository<Customer>();
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
