namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Xunit;
    using Xunit.Abstractions;

    public class RepositoryFactoryTests : TestBase
    {
        public RepositoryFactoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Create()
        {
            ForAllRepositoryFactories(TestCreate);
        }

        private static void TestCreate(IRepositoryFactory repoFactory)
        {
            Assert.NotNull(repoFactory.Create<Customer>());
            Assert.NotNull(repoFactory.Create<Customer, int>());
            Assert.NotNull(repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>());
            Assert.NotNull(repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>());
            Assert.NotNull(repoFactory.CreateInstance<Repository<Customer>>());
        }
    }
}
