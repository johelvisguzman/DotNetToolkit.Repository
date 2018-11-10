namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries.Strategies;
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class CompositeServiceTests : TestBase
    {
        public CompositeServiceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void GetWithTwoCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestGetWithTwoCompositePrimaryKey);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKey()
        {
            ForAllUnitOfWorkFactories(TestGetWithThreeCompositePrimaryKey);
        }

        [Fact]
        public void GetWithTwoCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void GetWithThreeCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestGetWithThreeCompositePrimaryKeyAsync);
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
        public void DeleteWithTwoCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithTwoCompositePrimaryKeyAsync);
        }

        [Fact]
        public void DeleteWithThreeCompositePrimaryKeyAsync()
        {
            ForAllUnitOfWorkFactoriesAsync(TestDeleteWithThreeCompositePrimaryKeyAsync);
        }

        [Fact]
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        [Fact]
        public void ThrowsIfEntityCompositePrimaryKeyMissingOrdering()
        {
            ForAllUnitOfWorkFactories(TestThrowsIfEntityCompositePrimaryKeyMissingOrdering);
        }

        private static void TestGetWithTwoCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2));
            Assert.Null(repo.Get(key1, key2, fetchStrategy));

            repo.Create(entity);

            Assert.Null(repo.Get(key1, randomKey));
            Assert.Null(repo.Get(key1, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2));
            Assert.NotNull(repo.Get(key1, key2, fetchStrategy));
        }

        private static void TestGetWithThreeCompositePrimaryKey(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(repo.Get(key1, key2, key3));
            Assert.Null(repo.Get(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.Null(repo.Get(key1, key2, randomKey));
            Assert.Null(repo.Get(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(repo.Get(key1, key2, key3));
            Assert.NotNull(repo.Get(key1, key2, key3, fetchStrategy));
        }

        private static async Task TestGetWithTwoCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithTwoCompositePrimaryKey, int, string>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2));
            Assert.Null(await repo.GetAsync(key1, key2, fetchStrategy));

            await repo.CreateAsync(entity);

            Assert.Null(await repo.GetAsync(key1, randomKey));
            Assert.Null(await repo.GetAsync(key1, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2));
            Assert.NotNull(await repo.GetAsync(key1, key2, fetchStrategy));
        }

        private static async Task TestGetWithThreeCompositePrimaryKeyAsync(IUnitOfWorkFactory uowFactory)
        {
            var repo = new Service<CustomerWithThreeCompositePrimaryKey, int, string, int>(uowFactory);

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

            Assert.Null(await repo.GetAsync(key1, key2, key3));
            Assert.Null(await repo.GetAsync(key1, key2, key3, fetchStrategy));

            repo.Create(entity);

            Assert.Null(await repo.GetAsync(key1, key2, randomKey));
            Assert.Null(await repo.GetAsync(key1, key2, randomKey, fetchStrategy));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3));
            Assert.NotNull(await repo.GetAsync(key1, key2, key3, fetchStrategy));
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

        private static void TestThrowsIfEntityPrimaryKeyTypesMismatch(IUnitOfWorkFactory uowFactory)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, int, string>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKey, string, int>(uowFactory));
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.Message);
        }

        private static void TestThrowsIfEntityCompositePrimaryKeyMissingOrdering(IUnitOfWorkFactory uowFactory)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Service<CustomerWithThreeCompositePrimaryKeyAndNoOrder, int, string, int>(uowFactory));
            Assert.Equal($"Unable to determine composite primary key ordering for type '{typeof(CustomerWithThreeCompositePrimaryKeyAndNoOrder).FullName}'. Use the ColumnAttribute to specify an order for composite primary keys.", ex.Message);
        }
    }
}
