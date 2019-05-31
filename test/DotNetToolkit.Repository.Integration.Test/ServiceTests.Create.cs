namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void Create()
        {
            ForAllServiceFactories(TestCreate);
        }

        [Fact]
        public void CreateWithSeededIdForIdentity()
        {
            ForAllServiceFactories(TestCreateWithSeededIdForIdentity);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentity()
        {
            ForAllServiceFactories(TestCreateWithSeededIdForNoneIdentity);
        }

        [Fact]
        public void CreateRange()
        {
            ForAllServiceFactories(TestCreateRange);
        }

        [Fact]
        public void CreateAsync()
        {
            ForAllServiceFactoriesAsync(TestCreateAsync);
        }

        [Fact]
        public void CreateWithSeededIdForIdentityAsync()
        {
            ForAllServiceFactoriesAsync(TestCreateWithSeededIdForIdentityAsync);
        }

        [Fact]
        public void CreateWithSeededIdForNoneIdentityAsync()
        {
            ForAllServiceFactoriesAsync(TestCreateWithSeededIdForNoneIdentityAsync);
        }

        [Fact]
        public void CreateRangeAsync()
        {
            ForAllServiceFactoriesAsync(TestCreateRangeAsync);
        }


        private static void TestCreate(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(service.GetExists(x => x.Name.Equals(name)));

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestCreateWithSeededIdForIdentity(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static void TestCreateWithSeededIdForNoneIdentity(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<CustomerWithNoIdentity>();

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(service.GetExists(key));

            service.Create(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(service.GetExists(key));
        }

        private static void TestCreateRange(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

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

        private static async Task TestCreateAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestCreateWithSeededIdForIdentityAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static async Task TestCreateWithSeededIdForNoneIdentityAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<CustomerWithNoIdentity>();

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(await service.GetExistsAsync(key));

            await service.CreateAsync(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(await service.GetExistsAsync(key));
        }

        private static async Task TestCreateRangeAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

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
