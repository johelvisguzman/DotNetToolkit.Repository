namespace DotNetToolkit.Repository.Integration.Test.Service
{
    using Data;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void Update()
        {
            ForAllServiceFactories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllServiceFactories(TestUpdateRange);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllServiceFactoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllServiceFactoriesAsync(TestUpdateRangeAsync);
        }

        private static void TestUpdate(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer {Name = name};

            service.Create(entity);

            var entityInDb = service.Get(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            service.Update(entityInDb);

            Assert.True(service.GetExists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer {Name = name},
                new Customer {Name = name}
            };

            service.Create(entities);

            var entitiesInDb = service.GetAll(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            service.Update(entitiesInDb);

            Assert.Equal(2, service.GetCount(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer {Name = name};

            await service.CreateAsync(entity);

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await service.UpdateAsync(entityInDb);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer {Name = name},
                new Customer {Name = name}
            };

            await service.CreateAsync(entities);

            var entitiesInDb = await service.GetAllAsync(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            await service.UpdateAsync(entitiesInDb);

            Assert.Equal(2, await service.GetCountAsync(x => x.Name.Equals(expectedName)));
        }
    }
}