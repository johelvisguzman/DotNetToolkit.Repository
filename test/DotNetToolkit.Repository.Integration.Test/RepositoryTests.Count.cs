namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Queries;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Count()
        {
            ForAllRepositoryFactories(TestCount);
        }

        [Fact]
        public void CountAsync()
        {
            ForAllRepositoryFactoriesAsync(TestCountAsync);
        }

        private static void TestCount(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };

            Assert.Equal(0, repo.Count());
            Assert.Equal(0, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(0, repo.Count(options));

            repo.Add(entity);

            Assert.Equal(1, repo.Count());
            Assert.Equal(1, repo.Count(x => x.Name.Equals(name)));
            Assert.Equal(1, repo.Count(options));
        }

        private static async Task TestCountAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entity = new Customer { Id = 1, Name = name };

            Assert.Equal(0, await repo.CountAsync());
            Assert.Equal(0, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(0, await repo.CountAsync(options));

            await repo.AddAsync(entity);

            Assert.Equal(1, await repo.CountAsync());
            Assert.Equal(1, await repo.CountAsync(x => x.Name.Equals(name)));
            Assert.Equal(1, await repo.CountAsync(options));
        }
    }
}
