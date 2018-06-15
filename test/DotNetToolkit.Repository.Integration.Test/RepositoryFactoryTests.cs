namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Xunit;

    public class RepositoryFactoryTests : TestBase
    {
        [Fact]
        public void Create()
        {
            ForAllRepositoryFactories(TestCreate);
        }

        private static void TestCreate(IRepositoryFactory repoFactory)
        {
            Assert.NotNull(repoFactory.Create<Customer>());
            Assert.NotNull(repoFactory.Create<Customer, int>());
        }
    }
}
