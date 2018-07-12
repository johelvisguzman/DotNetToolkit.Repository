namespace DotNetToolkit.Repository.Integration.Test
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Factories;
    using FetchStrategies;
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


        private static void TestGetWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;
            int randomKey = 3;

            var fetchStrategy = new FetchStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            var ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().Get(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().Get(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().Get(1, 1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().Get(1, 1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(repo.Get(key1, key2));
            Assert.Null(repo.Get(key1, key2, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Get(key1, randomKey));
            Assert.Null(repo.Get(key1, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2));
            Assert.NotNull(repo.Get(key1, key2, fetchStrategy));
        }

        private static void TestGetWithThreeCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;
            int randomKey = 4;

            var fetchStrategy = new FetchStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            var ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().Get(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().Get(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().Get(1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().Get(1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(repo.Get(key1, key2, key3));
            Assert.Null(repo.Get(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(repo.Get(key1, key2, randomKey));
            Assert.Null(repo.Get(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2, key3));
            Assert.NotNull(repo.Get(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestGetWithTwoCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int>();

            int key1 = 1;
            int key2 = 2;
            int randomKey = 3;

            var fetchStrategy = new FetchStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            var ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().GetAsync(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int>().GetAsync(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().GetAsync(1, 1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAnyAsync<ArgumentException>(() => repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, int, int>().GetAsync(1, 1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(await repo.GetAsync(key1, key2));
            Assert.Null(await repo.GetAsync(key1, key2, fetchStrategy));

            await repo.AddAsync(entity);

            Assert.Null(await repo.GetAsync(key1, randomKey));
            Assert.Null(await repo.GetAsync(key1, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2));
            Assert.NotNull(await repo.GetAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestGetWithThreeCompositePrimaryKeyAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int, int>();

            int key1 = 1;
            int key2 = 2;
            int key3 = 3;
            int randomKey = 4;

            var fetchStrategy = new FetchStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().GetAsync(1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int>().GetAsync(1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().GetAsync(1, 1));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            ex = await Assert.ThrowsAsync<ArgumentException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, int>().GetAsync(1, 1, fetchStrategy));
            Assert.Equal("The number of primary key values passed must match number of primary key values defined on the entity.\r\nParameter name: keyValues", ex.Message);

            Assert.Null(await repo.GetAsync(key1, key2, key3));
            Assert.Null(await repo.GetAsync(key1, key2, key3, fetchStrategy));

            repo.Add(entity);

            Assert.Null(await repo.GetAsync(key1, key2, randomKey));
            Assert.Null(await repo.GetAsync(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3, fetchStrategy));
        }
    }
}
