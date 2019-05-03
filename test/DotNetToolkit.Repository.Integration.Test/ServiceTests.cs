namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
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
    }
}
