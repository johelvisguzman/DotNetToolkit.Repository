namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void GetCount()
        {
            ForAllUnitOfWorkFactories(TestGetCount);
        }

        [Fact]
        public void GetCountAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetCountAsync);
        }

        private static void TestGetCount(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Name = name};

            Assert.Equal(0, service.GetCount());
            Assert.Equal(0, service.GetCount(x => x.Name.Equals(name)));
            Assert.Equal(0, service.GetCount(options));

            service.Create(entity);

            Assert.Equal(1, service.GetCount());
            Assert.Equal(1, service.GetCount(x => x.Name.Equals(name)));
            Assert.Equal(1, service.GetCount(options));
        }

        private static async Task TestGetCountAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer {Name = name};

            Assert.Equal(0, await service.GetCountAsync());
            Assert.Equal(0, await service.GetCountAsync(x => x.Name.Equals(name)));
            Assert.Equal(0, await service.GetCountAsync(options));

            await service.CreateAsync(entity);

            Assert.Equal(1, await service.GetCountAsync());
            Assert.Equal(1, await service.GetCountAsync(x => x.Name.Equals(name)));
            Assert.Equal(1, await service.GetCountAsync(options));
        }
    }
}