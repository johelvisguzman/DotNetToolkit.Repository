namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void Update()
        {
            ForAllRepositoryFactories(TestUpdate);
        }

        [Fact]
        public void UpdateRange()
        {
            ForAllRepositoryFactories(TestUpdateRange);
        }

        [Fact]
        public void UpdateAsync()
        {
            ForAllRepositoryFactoriesAsync(TestUpdateAsync);
        }

        [Fact]
        public void UpdateRangeAsync()
        {
            ForAllRepositoryFactoriesAsync(TestUpdateRangeAsync);
        }

        private static void TestUpdate(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            repo.Add(entity);

            var entityInDb = repo.Find(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            repo.Update(entityInDb);

            Assert.True(repo.Exists(x => x.Name.Equals(expectedName)));
        }

        private static void TestUpdateRange(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            repo.Add(entities);

            var entitiesInDb = repo.FindAll(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            repo.Update(entitiesInDb);

            Assert.Equal(2, repo.Count(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entity = new Customer { Name = name };

            await repo.AddAsync(entity);

            var entityInDb = await repo.FindAsync(x => x.Name.Equals(name));

            entityInDb.Name = expectedName;

            await repo.UpdateAsync(entityInDb);

            Assert.True(await repo.ExistsAsync(x => x.Name.Equals(expectedName)));
        }

        private static async Task TestUpdateRangeAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string expectedName = "New Random Name";
            const string name = "Random Name";

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = name },
                new Customer { Id = 2, Name = name }
            };

            await repo.AddAsync(entities);

            var entitiesInDb = await repo.FindAllAsync(x => x.Name.Equals(name));

            foreach (var entityInDb in entitiesInDb)
            {
                entityInDb.Name = expectedName;
            }

            await repo.UpdateAsync(entitiesInDb);

            Assert.Equal(2, await repo.CountAsync(x => x.Name.Equals(expectedName)));
        }
    }
}
