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
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        [Fact]
        public void ThrowsIfEntityCompositePrimaryKeyMissingOrdering()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityCompositePrimaryKeyMissingOrdering);
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
