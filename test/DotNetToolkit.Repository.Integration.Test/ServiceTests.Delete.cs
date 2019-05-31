namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void Delete()
        {
            ForAllServiceFactories(TestDelete);
        }

        [Fact]
        public void DeleteWithId()
        {
            ForAllServiceFactories(TestDeleteWithId);
        }

        [Fact]
        public void TryDelete()
        {
            ForAllServiceFactories(TestTryDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllServiceFactories(TestDeleteRange);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKey()
        {
            ForAllServiceFactories(TestDeleteWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKey()
        {
            ForAllServiceFactories(TestDeleteWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void DeleteAsync()
        {
            ForAllServiceFactoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteWithIdAsync()
        {
            ForAllServiceFactoriesAsync(TestDeleteWithIdAsync);
        }

        [Fact]
        public void TryDeleteAsync()
        {
            ForAllServiceFactoriesAsync(TestTryDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllServiceFactoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKeyAsync()
        {
            ForAllServiceFactoriesAsync(TestDeleteWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKeyAsync()
        {
            ForAllServiceFactoriesAsync(TestDeleteWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestDelete(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer {Name = name};

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));

            var entityInDb = service.Get(x => x.Name.Equals(name));

            service.Delete(entityInDb);

            Assert.False(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteWithId(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(service.GetExists(id));

            var ex = Assert.Throws<InvalidOperationException>(() => service.Delete(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            service.Create(entity);

            Assert.True(service.GetExists(id));

            service.Delete(id);

            Assert.False(service.GetExists(id));
        }

        private static void TestTryDelete(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(service.TryDelete(id));

            service.Create(entity);

            Assert.True(service.TryDelete(id));
        }

        private static void TestDeleteRange(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer {Name = name},
                new Customer {Name = name}
            };

            service.Create(entities);

            Assert.Equal(2, service.GetCount());

            var entitiesInDb = service.GetAll(x => x.Name.Equals(name));

            service.Delete(entitiesInDb);

            Assert.Equal(0, service.GetCount());
        }

        private static void TestDeleteWithTwoCompositePrimaryKey(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(repo.GetExists(key1, key2));

            repo.Create(entity);

            Assert.True(repo.GetExists(key1, key2));

            repo.Delete(key1, key2);

            Assert.False(repo.GetExists(key1, key2));
        }

        private static void TestDeleteWithThreeCompositePrimaryKey(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(repo.GetExists(key1, key2, key3));

            repo.Create(entity);

            Assert.True(repo.GetExists(key1, key2, key3));

            repo.Delete(key1, key2, key3);

            Assert.False(repo.GetExists(key1, key2, key3));
        }

        private static async Task TestDeleteAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer {Name = name};

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            await service.DeleteAsync(entityInDb);

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteWithIdAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(await service.GetExistsAsync(id));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(id));

            await service.DeleteAsync(id);

            Assert.False(await service.GetExistsAsync(id));
        }

        private static async Task TestTryDeleteAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(await service.TryDeleteAsync(id));

            await service.CreateAsync(entity);

            Assert.True(await service.TryDeleteAsync(id));
        }

        private static async Task TestDeleteRangeAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer {Name = name},
                new Customer {Name = name}
            };

            await service.CreateAsync(entities);

            Assert.Equal(2, await service.GetCountAsync());

            var entitiesInDb = await service.GetAllAsync(x => x.Name.Equals(name));

            await service.DeleteAsync(entitiesInDb);

            Assert.Equal(0, await service.GetCountAsync());
        }

        private static async Task TestDeleteWithTwoCompositePrimaryKeyAsync(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(await repo.GetExistsAsync(key1, key2));

            await repo.CreateAsync(entity);

            Assert.True(await repo.GetExistsAsync(key1, key2));

            await repo.DeleteAsync(key1, key2);

            Assert.False(await repo.GetExistsAsync(key1, key2));
        }

        private static async Task TestDeleteWithThreeCompositePrimaryKeyAsync(IServiceFactory serviceFactory)
        {
            var repo = serviceFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(await repo.GetExistsAsync(key1, key2, key3));

            await repo.CreateAsync(entity);

            Assert.True(await repo.GetExistsAsync(key1, key2, key3));

            await repo.DeleteAsync(key1, key2, key3);

            Assert.False(await repo.GetExistsAsync(key1, key2, key3));
        }
    }
}