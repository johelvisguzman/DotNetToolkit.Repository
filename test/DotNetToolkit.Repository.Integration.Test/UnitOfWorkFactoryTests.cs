namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Transactions;
    using Xunit;

    public class UnitOfWorkFactoryTests : TestBase
    {
        [Fact]
        public void Create()
        {
            ForAllUnitOfWorkFactories(TestCreate);
        }

        private static void TestCreate(IUnitOfWorkFactory uowFactory)
        {
            Assert.NotNull(uowFactory.Create());
            Assert.NotNull(uowFactory.CreateInstance<UnitOfWork>());
        }
    }
}
