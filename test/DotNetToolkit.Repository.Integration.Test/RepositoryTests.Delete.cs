namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Delete()
        {
            ForAllRepositoryFactories(TestDelete);
        }

        [Fact]
        public void DeleteWithId()
        {
            ForAllRepositoryFactories(TestDeleteWithId);
        }

        [Fact]
        public void TryDelete()
        {
            ForAllRepositoryFactories(TestTryDelete);
        }

        [Fact]
        public void DeleteRange()
        {
            ForAllRepositoryFactories(TestDeleteRange);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestDeleteWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestDeleteWithThreeCompositePrimaryKey);
        }
        
        [Fact]
        public void DeleteAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteAsync);
        }

        [Fact]
        public void DeleteWithIdAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteWithIdAsync);
        }

        [Fact]
        public void TryDeleteAsync()
        {
            ForAllRepositoryFactoriesAsync(TestTryDeleteAsync);
        }

        [Fact]
        public void DeleteRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteRangeAsync);
        }

        [Fact]
        public void DeleteWithTwoCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestDelete(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            repo.Delete(entityInDb);

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestDeleteWithId(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(repo.Exists(id));

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            repo.Add(entity);

            Assert.True(repo.Exists(id));

            repo.Delete(id);

            Assert.False(repo.Exists(id));
        }
        private static void TestTryDelete(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(repo.TryDelete(id));

            repo.Add(entity);

            Assert.True(repo.TryDelete(id));
        }

        private static void TestDeleteRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            repo.Add(entities);

            Assert.Equal(2, repo.Count());

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            repo.Delete(entitiesInDb);

            Assert.Equal(0, repo.Count());
        }

        private static async Task TestDeleteAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));

            var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

            await repo.DeleteAsync(entityInDb);

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestDeleteWithIdAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await repo.ExistsAsync(id));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.DeleteAsync(id));
            Assert.Equal($"No entity found in the repository with the '{id}' key.", ex.Message);

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(id));

            await repo.DeleteAsync(id);

            Assert.False(await repo.ExistsAsync(id));
        }

        private static async Task TestTryDeleteAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await repo.TryDeleteAsync(id));

            await repo.AddAsync(entity);

            Assert.True(await repo.TryDeleteAsync(id));
        }

        private static async Task TestDeleteRangeAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            await repo.AddAsync(entities);

            Assert.Equal(2, await repo.CountAsync());

            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

            await repo.DeleteAsync(entitiesInDb);

            Assert.Equal(0, await repo.CountAsync());
        }

        private static void TestDeleteWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(repo.Exists(key1, key2));

            repo.Add(entity);

            Assert.True(repo.Exists(key1, key2));

            repo.Delete(key1, key2);

            Assert.False(repo.Exists(key1, key2));
        }

        private static void TestDeleteWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(repo.Exists(key1, key2, key3));

            repo.Add(entity);

            Assert.True(repo.Exists(key1, key2, key3));

            repo.Delete(key1, key2, key3);

            Assert.False(repo.Exists(key1, key2, key3));
        }

        private static async Task TestDeleteWithTwoCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(await repo.ExistsAsync(key1, key2));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(key1, key2));

            await repo.DeleteAsync(key1, key2);

            Assert.False(await repo.ExistsAsync(key1, key2));
        }

        private static async Task TestDeleteWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(await repo.ExistsAsync(key1, key2, key3));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(key1, key2, key3));

            await repo.DeleteAsync(key1, key2, key3);

            Assert.False(await repo.ExistsAsync(key1, key2, key3));
        }
    }
}
