namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Xunit;

    public class UnitOfWorkFactoryTests : TestBase
    {
        [Fact]
        public void Create()
        {
            ForAllUnitOfWorkFactories(TestCreate);
        }

        private static void TestCreate(IUnitOfWorkFactory repoFactory)
        {
            Assert.NotNull(repoFactory.Create());
        }
    }
}
