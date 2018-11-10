namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries.Strategies;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class CompositeRepositoryTests : TestBase
    {
        public CompositeRepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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

        [Fact]
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllRepositoryFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        [Fact]
        public void ThrowsIfEntityCompositePrimaryKeyMissingOrdering()
        {
            ForAllRepositoryFactories(TestThrowsIfEntityCompositePrimaryKeyMissingOrdering);
        }

        private static void TestGetWithTwoCompositePrimaryKey(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

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
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

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
            var repo = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();

            var key1 = 1;
            var key2 = "2";
            var randomKey = "3";

            var fetchStrategy = new FetchQueryStrategy<CustomerWithTwoCompositePrimaryKey>();

            var entity = new CustomerWithTwoCompositePrimaryKey { Id1 = key1, Id2 = key2, Name = "Random Name" };

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
            var repo = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();

            var key1 = 1;
            var key2 = "2";
            var key3 = 3;
            var randomKey = 4;

            var fetchStrategy = new FetchQueryStrategy<CustomerWithThreeCompositePrimaryKey>();

            var entity = new CustomerWithThreeCompositePrimaryKey { Id1 = key1, Id2 = key2, Id3 = key3, Name = "Random Name" };

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

        private static void TestThrowsIfEntityPrimaryKeyTypesMismatch(IRepositoryFactory repoFactory)
        {
            var ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, string>());
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.InnerException?.Message);

            ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, string, string>());
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.InnerException?.Message);

            ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string>());
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.InnerException?.Message);

            ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, string, int>());
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.InnerException?.Message);
        }

        private static void TestThrowsIfEntityCompositePrimaryKeyMissingOrdering(IRepositoryFactory repoFactory)
        {
            var ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKeyAndNoOrder, int, string, int>());
            Assert.Equal($"Unable to determine composite primary key ordering for type '{typeof(CustomerWithThreeCompositePrimaryKeyAndNoOrder).FullName}'. Use the ColumnAttribute to specify an order for composite primary keys.", ex.InnerException?.Message);
        }
    }
}
