namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using Data;
    using Fixtures;
    using Query;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public partial class RepositoryTests : TestBase, IClassFixture<RepositoryTestsFixture>
    {
        public RepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
#if NETCORE
            Running.AzureStorageEmulatorManager.Clear();
#endif
        }

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
        public void ThrowsIfEntityHasNoIdOnContextCreation()
        {
            ForAllRepositoryFactories(TestThrowsIfEntityHasNoIdOnContextCreation);
        }

        [Fact]
        public void ThrowsIfContextProviderNotConfiguered()
        {
            var options = new RepositoryOptionsBuilder().Options;
            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer>(options));

            Assert.Equal("No context provider has been configured. For more information on DotNetToolkit.Repository options configuration, visit the https://github.com/johelvisguzman/DotNetToolkit.Repository/wiki/Repository-Options-Configuration.", ex.Message);
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

        private static void TestThrowsIfEntityHasNoIdOnContextCreation(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithNoId>();

            var method = typeof(InternalRepositoryBase<CustomerWithNoId>)
                .GetTypeInfo()
                .GetDeclaredMethod("GetContext"); // protected method

            var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(repo, null));

            Assert.Equal($"The instance of entity type '{typeof(CustomerWithNoId).FullName}' requires a primary key to be defined.", ex.InnerException?.Message);
        }
    }
}
