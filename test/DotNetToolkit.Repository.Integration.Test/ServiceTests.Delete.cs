namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
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
            ForAllUnitOfWorkFactories(TestDelete);
        }

        [Fact]
        public void DeleteWithId()
        {
            ForAllUnitOfWorkFactories(TestDeleteWithId);
        }

        [Fact]
        public void TryDelete()
        {
            ForAllUnitOfWorkFactories(TestTryDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllUnitOfWorkFactories(TestDeleteRange);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestDeleteWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestDeleteWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void DeleteAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteWithIdAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithIdAsync);
        }

        [Fact]
        public void TryDeleteAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestTryDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestDelete(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer {Name = name};

            service.Create(entity);

            Assert.True(service.GetExists(x => x.Name.Equals(name)));

            var entityInDb = service.Get(x => x.Name.Equals(name));

            service.Delete(entityInDb);

            Assert.False(service.GetExists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteWithId(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestTryDelete(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(service.TryDelete(id));

            service.Create(entity);

            Assert.True(service.TryDelete(id));
        }

        private static void TestDeleteRange(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static void TestDeleteWithTwoCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(repo.GetExists(key1, key2));

            repo.Create(entity);

            Assert.True(repo.GetExists(key1, key2));

            repo.Delete(key1, key2);

            Assert.False(repo.GetExists(key1, key2));
        }

        private static void TestDeleteWithThreeCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

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

        private static async Task TestDeleteAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const string name = "Random Name";

            var entity = new Customer {Name = name};

            await service.CreateAsync(entity);

            Assert.True(await service.GetExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = await service.GetAsync(x => x.Name.Equals(name));

            await service.DeleteAsync(entityInDb);

            Assert.False(await service.GetExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteWithIdAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestTryDeleteAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

            const int id = 1;

            var entity = new Customer {Id = id};

            Assert.False(await service.TryDeleteAsync(id));

            await service.CreateAsync(entity);

            Assert.True(await service.TryDeleteAsync(id));
        }

        private static async Task TestDeleteRangeAsync(IUnitOfWorkFactory uowFactory)
        {
            var service = new Service<Customer>(uowFactory);

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

        private static async Task TestDeleteWithTwoCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(await repo.GetExistsAsync(key1, key2));

            await repo.CreateAsync(entity);

            Assert.True(await repo.GetExistsAsync(key1, key2));

            await repo.DeleteAsync(key1, key2);

            Assert.False(await repo.GetExistsAsync(key1, key2));
        }

        private static async Task TestDeleteWithThreeCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

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