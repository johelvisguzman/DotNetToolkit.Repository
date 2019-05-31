namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Services;
    using Xunit;
    using Xunit.Abstractions;

    public partial class ServiceTests : TestBase
    {
        public ServiceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void FactoryCreate()
        {
            ForAllServiceFactories(TestFactoryCreate);
        }

        [Fact]
        public void AsReadOnly()
        {
            ForAllServiceFactories(TestAsReadOnly);
        }               

        private static void TestFactoryCreate(IServiceFactory serviceFactory)
        {
            Assert.NotNull(serviceFactory.Create<Customer>());
            Assert.NotNull(serviceFactory.Create<Customer, int>());
            Assert.NotNull(serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>());
            Assert.NotNull(serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>());
            Assert.NotNull(serviceFactory.CreateInstance<Service<Customer>>());
        }

        private static void TestAsReadOnly(IServiceFactory serviceFactory)
        {
            var service1 = serviceFactory.Create<Customer>();
            var readOnlyRepo1 = service1.AsReadOnly();

            Assert.Equal(readOnlyRepo1, service1.AsReadOnly());

            var service2 = serviceFactory.Create<Customer, int>();
            var readOnlyRepo2 = service2.AsReadOnly();

            Assert.Equal(readOnlyRepo2, service2.AsReadOnly());

            var service3 = serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();
            var readOnlyRepo3 = service3.AsReadOnly();

            Assert.Equal(readOnlyRepo3, service3.AsReadOnly());

            var service4 = serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();
            var readOnlyRepo4 = service4.AsReadOnly();

            Assert.Equal(readOnlyRepo4, service4.AsReadOnly());
        }
    }
}
