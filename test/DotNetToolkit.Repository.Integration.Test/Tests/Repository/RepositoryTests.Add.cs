namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using Data;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Add()
        {
            ForAllRepositoryFactories(TestAdd);
        }

        [Fact]
        public void AddWithSeededIdForIdentity()
        {
            ForAllRepositoryFactories(TestAddWithSeededIdForIdentity, 
                ContextProviderType.EntityFrameworkCore, 
                ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void AddWithSeededIdForNoneIdentity()
        {
            ForAllRepositoryFactories(TestAddWithSeededIdForNoneIdentity);
        }

        [Fact]
        public void AddRange()
        {
            ForAllRepositoryFactories(TestAddRange);
        }

        [Fact]
        public void AddAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddAsync);
        }

        [Fact]
        public void AddWithSeededIdForIdentityAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddWithSeededIdForIdentityAsync, 
                ContextProviderType.EntityFrameworkCore, 
                ContextProviderType.AzureStorageBlob);
        }

        [Fact]
        public void AddWithSeededIdForNoneIdentityAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddWithSeededIdForNoneIdentityAsync);
        }

        [Fact]
        public void AddRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestAddRangeAsync);
        }

        private static void TestAdd(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Id = 1, Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
        }

        private static void TestAddWithSeededIdForIdentity(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(repo.Exists(key));

            repo.Add(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static void TestAddWithSeededIdForNoneIdentity(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithNoIdentity>();

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(repo.Exists(key));

            repo.Add(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(repo.Exists(key));
        }

        private static void TestAddRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            Assert.Equal(0, repo.Count());

            repo.Add(entities);

            Assert.Equal(2, repo.Count());
        }

        private static async Task TestAddAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entity = new Customer { Id = 1, Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
        }

        private static async Task TestAddWithSeededIdForIdentityAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int key = 9;

            var entity = new Customer { Id = key };

            Assert.False(await repo.ExistsAsync(key));

            await repo.AddAsync(entity);

            // should be one since it is autogenerating identity models
            Assert.Equal(1, entity.Id);
        }

        private static async Task TestAddWithSeededIdForNoneIdentityAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<CustomerWithNoIdentity>();

            const int key = 9;

            var entity = new CustomerWithNoIdentity { Id = key };

            Assert.False(await repo.ExistsAsync(key));

            await repo.AddAsync(entity);

            Assert.Equal(key, entity.Id);
            Assert.True(await repo.ExistsAsync(key));
        }

        private static async Task TestAddRangeAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            Assert.Equal(0, await repo.CountAsync());

            await repo.AddAsync(entities);

            Assert.Equal(2, await repo.CountAsync());
        }
    }
}
