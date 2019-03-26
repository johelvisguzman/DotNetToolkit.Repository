namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using System;
    using Xunit;
    using Xunit.Abstractions;

    public partial class ServiceTests : TestBase
    {
        public ServiceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void AsReadOnly()
        {
            ForAllUnitOfWorkFactories(TestAsReadOnly);
        }

        [Fact]
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        [Fact]
        public void ThrowsIfEntityCompositePrimaryKeyMissingOrdering()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityCompositePrimaryKeyMissingOrdering);
        }

        private static void TestAsReadOnly(IUnitOfWorkFactory uowFactory)
        {
            var service1 = new Service<Customer>(uowFactory);
            var readOnlyRepo1 = service1.AsReadOnly();

            Assert.Equal(readOnlyRepo1, service1.AsReadOnly());

            var service2 = new Service<Customer, int>(uowFactory);
            var readOnlyRepo2 = service2.AsReadOnly();

            Assert.Equal(readOnlyRepo2, service2.AsReadOnly());

            var service3 = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);
            var readOnlyRepo3 = service3.AsReadOnly();

            Assert.Equal(readOnlyRepo3, service3.AsReadOnly());

            var service4 = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);
            var readOnlyRepo4 = service4.AsReadOnly();

            Assert.Equal(readOnlyRepo4, service4.AsReadOnly());
        }

        private static void TestThrowsIfEntityPrimaryKeyTypesMismatch(IUnitOfWorkFactory uowFactory)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, int, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string, int>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);
        }

        private static void TestThrowsIfEntityCompositePrimaryKeyMissingOrdering(IUnitOfWorkFactory uowFactory)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKeyAndNoOrder, int, string, int>(uowFactory));
            Assert.Equal($"Unable to determine composite primary key ordering for type '{typeof(CustomerWithThreeCompositePrimaryKeyAndNoOrder).FullName}'. Use the ColumnAttribute to specify an order for composite primary keys.", ex.Message);
        }
    }
}
