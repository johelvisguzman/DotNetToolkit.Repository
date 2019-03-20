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
        public void Update()
        {
            ForAllUnitOfWorkFactories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllUnitOfWorkFactories(TestUpdateRange);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestUpdateRangeAsync);
        }

        private static void TestUpdate(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer {Name = name};

            service.Create(entity);

            var entityInDb = service.Get(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            service.Update(entityInDb);

            Assert.True(service.GetExists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestUpdateAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer {Name = name};

            await service.CreateAsync(entity);

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await service.UpdateAsync(entityInDb);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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