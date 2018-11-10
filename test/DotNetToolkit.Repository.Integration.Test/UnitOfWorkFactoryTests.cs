namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class UnitOfWorkFactoryTests : TestBase
    {
        public UnitOfWorkFactoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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
