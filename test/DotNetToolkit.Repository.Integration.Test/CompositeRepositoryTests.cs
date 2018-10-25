namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries.Strategies;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CompositeRepositoryTests : TestBase
    {
        [Fact]
        public void GetWithTwoCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestGetWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKey()
        {
            ForAllRepositoryFactories(TestGetWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGetWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGetWithThreeCompositePrimaryKeyAsync);
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
        public void DeleteWithTwoCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKeyAsync()
        {
            ForAllRepositoryFactoriesAsync(TestDeleteWithThreeCompositePrimaryKeyAsync);
        }

        private static void TestGetWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;
            int randomKey = 3;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            var ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().Find(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().Find(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().Find(1, 1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().Find(1, 1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(repo.Find(key1, key2));
            Assert.Null(repo.Find(key1, key2, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Find(key1, randomKey));
            Assert.Null(repo.Find(key1, randomKey, fetchStrategy));
            Assert.NotNull(repo.Find(key1, key2));
            Assert.NotNull(repo.Find(key1, key2, fetchStrategy));
        }

        private static void TestGetWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;
            int randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            var ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().Find(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().Find(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().Find(1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().Find(1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(repo.Find(key1, key2, key3));
            Assert.Null(repo.Find(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Find(key1, key2, randomKey));
            Assert.Null(repo.Find(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(repo.Find(key1, key2, key3));
            Assert.NotNull(repo.Find(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestGetWithTwoCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;
            int randomKey = 3;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            var ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().FindAsync(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().FindAsync(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().FindAsync(1, 1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().FindAsync(1, 1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(await repo.FindAsync(key1, key2));
            Assert.Null(await repo.FindAsync(key1, key2, fetchStrategy));

            await repo.AddAsync(entity);

            Assert.Null(await repo.FindAsync(key1, randomKey));
            Assert.Null(await repo.FindAsync(key1, randomKey, fetchStrategy));
            Assert.NotNull(await repo.FindAsync(key1, key2));
            Assert.NotNull(await repo.FindAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestGetWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;
            int randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().FindAsync(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().FindAsync(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().FindAsync(1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().FindAsync(1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(await repo.FindAsync(key1, key2, key3));
            Assert.Null(await repo.FindAsync(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(await repo.FindAsync(key1, key2, randomKey));
            Assert.Null(await repo.FindAsync(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(await repo.FindAsync(key1, key2, key3));
            Assert.NotNull(await repo.FindAsync(key1, key2, key3, fetchStrategy));
        }

        private static void TestDeleteWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(repo.Exists(key1, key2));

            repo.Add(entity);

            Assert.True(repo.Exists(key1, key2));

            repo.Delete(key1, key2);

            Assert.False(repo.Exists(key1, key2));
        }

        private static void TestDeleteWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(repo.Exists(key1, key2, key3));

            repo.Add(entity);

            Assert.True(repo.Exists(key1, key2, key3));

            repo.Delete(key1, key2, key3);

            Assert.False(repo.Exists(key1, key2, key3));
        }

        private static async Task TestDeleteWithTwoCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.False(await repo.ExistsAsync(key1, key2));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(key1, key2));

            await repo.DeleteAsync(key1, key2);

            Assert.False(await repo.ExistsAsync(key1, key2));
        }

        private static async Task TestDeleteWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.False(await repo.ExistsAsync(key1, key2, key3));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(key1, key2, key3));

            await repo.DeleteAsync(key1, key2, key3);

            Assert.False(await repo.ExistsAsync(key1, key2, key3));
        }
    }
}
