namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Factories;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Exists()
        {
            ForAllRepositoryFactories(TestExists);
        }

        [Fact]
        public void ExistsWithId()
        {
            ForAllRepositoryFactories(TestExistsWithId);
        }

        [Fact]
        public void ExistsAsync()
        {
            ForAllRepositoryFactoriesAsync(TestExistsAsync);
        }

        [Fact]
        public void ExistsWithIdAsync()
        {
            ForAllRepositoryFactoriesAsync(TestExistsWithIdAsync);
        }

        private static void TestExists(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.False(repo.Exists(x => x.Name.Equals(name)));
            Assert.False(repo.Exists(options));

            repo.Add(entity);

            Assert.True(repo.Exists(x => x.Name.Equals(name)));
            Assert.True(repo.Exists(options));
        }

        private static void TestExistsWithId(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(repo.Exists(id));

            repo.Add(entity);

            Assert.True(repo.Exists(id));
        }

        private static async Task TestExistsAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>().SatisfyBy(x => x.Name.Equals(name));
            var entity = new Customer { Name = name };

            Assert.False(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.False(await repo.ExistsAsync(options));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(name)));
            Assert.True(await repo.ExistsAsync(options));
        }

        private static async Task TestExistsWithIdAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const int id = 1;

            var entity = new Customer { Id = id };

            Assert.False(await repo.ExistsAsync(id));

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(id));
        }
    }
}
