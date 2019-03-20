namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void Create()
        {
            ForAllUnitOfWorkFactories(TestCreate);
        }

        [Fact]
        public void CreateWithSeededIdForIdentity()
        {
            ForAllUnitOfWorkFactories(TestCreateWithSeededIdForIdentity);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentity()
        {
            ForAllUnitOfWorkFactories(TestCreateWithSeededIdForNoneIdentity);
        }

        [Fact]
        public void CreateRange()
        {
            ForAllUnitOfWorkFactories(TestCreateRange);
        }

        [Fact]
        public void CreateAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateAsync);
        }

        [Fact]
        public void CreateWithSeededIdForIdentityAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateWithSeededIdForIdentityAsync);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentityAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateWithSeededIdForNoneIdentityAsync);
        }

        [Fact]
        public void CreateRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestCreateRangeAsync);
        }


        private static void TestCreate(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(service.GetExists(x => x.Name.Equals(name)));

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestCreateWithSeededIdForIdentity(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static void TestCreateWithSeededIdForNoneIdentity(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<CustomerWithNoIdentity>(uowFactory);

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(service.GetExists(key));
        }

        private static void TestCreateRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, service.GetCount());

            service.Create(entities);

            Assert.Equal(2, service.GetCount());
        }

        private static async Task TestCreateAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestCreateWithSeededIdForIdentityAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static async Task TestCreateWithSeededIdForNoneIdentityAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<CustomerWithNoIdentity>(uowFactory);

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(await service.GetExistsAsync(key));
        }

        private static async Task TestCreateRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Name = name },
                new Customer { Name = name }
            };

            Assert.Equal(0, await service.GetCountAsync());

            await service.CreateAsync(entities);

            Assert.Equal(2, await service.GetCountAsync());
        }
    }
}
