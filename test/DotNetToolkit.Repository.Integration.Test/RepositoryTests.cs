namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public partial class RepositoryTests : TestBase
    {
        public RepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void FactoryCreate()
        {
            ForAllRepositoryFactories(TestFactoryCreate);
        }

        [Fact]
        public void AsReadOnly()
        {
            ForAllRepositoryFactories(TestAsReadOnly);
        }

        [Fact]
        public void ThrowsIfModelHasNoId()
        {
            ForAllRepositoryFactories(TestThrowsIfModelHasNoId);
        }

        [Fact]
        public void ThrowsIfEntityPrimaryKeyTypesMismatch()
        {
            ForAllRepositoryFactories(TestThrowsIfEntityPrimaryKeyTypesMismatch);
        }

        [Fact]
        public void ThrowsIfSpecificationMissingFromQueryOptions()
        {
            ForAllRepositoryFactories(TestThrowsIfSpecificationMissingFromQueryOptions);
        }

        [Fact]
        public void ThrowsIfSpecificationMissingFromQueryOptionsAsync()
        {
            ForAllRepositoryFactoriesAsync(TestThrowsIfSpecificationMissingFromQueryOptionsAsync);
        }

        [Fact]
        public void ThrowsIfEntityCompositePrimaryKeyMissingOrdering()
        {
            ForAllRepositoryFactories(TestThrowsIfEntityCompositePrimaryKeyMissingOrdering);
        }

        private static void TestFactoryCreate(IRepositoryFactory repoFactory)
        {
            Assert.NotNull(repoFactory.Create<Customer>());
            Assert.NotNull(repoFactory.Create<Customer, int>());
            Assert.NotNull(repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>());
            Assert.NotNull(repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>());
            Assert.NotNull(repoFactory.CreateInstance<Repository<Customer>>());
        }

        private static void TestAsReadOnly(IRepositoryFactory repoFactory)
        {
            var repo1 = repoFactory.Create<Customer>();
            var readOnlyRepo1 = repo1.AsReadOnly();

            Assert.Equal(readOnlyRepo1, repo1.AsReadOnly());

            var repo2 = repoFactory.Create<Customer, int>();
            var readOnlyRepo2 = repo2.AsReadOnly();

            Assert.Equal(readOnlyRepo2, repo2.AsReadOnly());

            var repo3 = repoFactory.Create<CustomerWithTwoCompositePrimaryKey, int, string>();
            var readOnlyRepo3 = repo3.AsReadOnly();

            Assert.Equal(readOnlyRepo3, repo3.AsReadOnly());

            var repo4 = repoFactory.Create<CustomerWithThreeCompositePrimaryKey, int, string, int>();
            var readOnlyRepo4 = repo4.AsReadOnly();

            Assert.Equal(readOnlyRepo4, repo4.AsReadOnly());
        }

        private static void TestThrowsIfModelHasNoId(IRepositoryFactory repoFactory)
        {
            var ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithNoId>());

            Assert.Equal($"The instance of entity type '{typeof(CustomerWithNoId).FullName}' requires a primary key to be defined.", ex.InnerException?.Message);
        }

        private static void TestThrowsIfSpecificationMissingFromQueryOptions(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var emptyQueryOptions = new QueryOptions<Customer>();

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Exists(emptyQueryOptions));
            Assert.Equal("The specified query options is missing a specification predicate.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(emptyQueryOptions));
            Assert.Equal("The specified query options is missing a specification predicate.", ex.Message);
        }

        private static async Task TestThrowsIfSpecificationMissingFromQueryOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();
            var emptyQueryOptions = new QueryOptions<Customer>();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.ExistsAsync(emptyQueryOptions));
            Assert.Equal("The specified query options is missing a specification predicate.", ex.Message);

            ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.DeleteAsync(emptyQueryOptions));
            Assert.Equal("The specified query options is missing a specification predicate.", ex.Message);
        }

        private static void TestThrowsIfEntityPrimaryKeyTypesMismatch(IRepositoryFactory repoFactory)
        {
            var ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<Customer, string>());
            Assert.Equal("The repository primary key type(s) constraint must match the number of primary key type(s) and ordering defined on the entity.", ex.InnerException?.Message);

            ex = Assert.Throws<TargetInvocationException>(() => repoFactory.Create<CustomerWithThreeCompositePrimaryKey, string>());
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
