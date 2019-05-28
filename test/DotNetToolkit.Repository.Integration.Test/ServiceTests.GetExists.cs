namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using Services;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void GetExists()
        {
            ForAllUnitOfWorkFactories(TestGetExists);
        }

        [Fact]
        public void GetExistsWithId()
        {
            ForAllUnitOfWorkFactories(TestGetExistsWithId);
        }

        [Fact]
        public void GetExistsAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetExistsAsync);
        }

        [Fact]
        public void GetExistsWithIdAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetExistsWithIdAsync);
        }

        private static void TestGetExists(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer {Name = name};

            Assert.False(service.GetExists(x => x.Name.Equals(name)));
            Assert.False(service.GetExists(options));

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));
            Assert.True(service.GetExists(options));
        }

        private static void TestGetExistsWithId(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(service.GetExists(id));

            service.Create(entity);

            Assert.True(service.GetExists(id));
        }

        private static async Task TestGetExistsAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer {Name = name};

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await service.GetExistsAsync(options));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await service.GetExistsAsync(options));
        }

        private static async Task TestGetExistsWithIdAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(await service.GetExistsAsync(id));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(id));
        }
    }
}