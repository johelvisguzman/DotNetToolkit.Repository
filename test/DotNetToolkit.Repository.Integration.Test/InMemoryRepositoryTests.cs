namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using InMemory;
    using System;
    using Xunit;

    public class InMemoryRepositoryTests : TestBase
    {
        [Fact]
        public void CanScoped()
        {
            ForAllRepositoriesInMemory(TestCanScoped);
        }

        [Fact]
        public void AddWithSeededId()
        {
            ForAllRepositoriesInMemory(TestAddWithSeededId);
        }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            ForAllRepositoriesInMemory(TestThrowsIfDeleteWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            ForAllRepositoriesInMemory(TestThrowsIfUpdateWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            ForAllRepositoriesInMemory(TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue);
        }

        private static void TestCanScoped(IRepository<Customer> repo)
        {
            var databaseName = Guid.NewGuid().ToString();

            using (repo = CreateRepositoryInstanceOfType(repo.GetType(), databaseName))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }

            using (repo = CreateRepositoryInstanceOfType(repo.GetType(), databaseName))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.Equal(2, entity.Id);
                Assert.Equal(2, repo.Count(x => x.Name.Equals("Random Name")));
            }

            using (repo = CreateRepositoryInstanceOfType(repo.GetType(), Guid.NewGuid().ToString()))
            {
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }
        }

        private static void TestAddWithSeededId(IRepository<Customer> repo)
        {
            const int expectedId = 9;

            var entity = new Customer { Id = expectedId, Name = "Random Name" };

            repo.Add(entity);

            Assert.NotNull(repo.Get(expectedId));
        }

        private static void TestThrowsIfDeleteWhenEntityNoInStore(IRepository<Customer> repo)
        {
            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        private static void TestThrowsIfUpdateWhenEntityNoInStore(IRepository<Customer> repo)
        {
            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        private static void TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue(IRepository<Customer> repo)
        {
            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the in-memory store because another instance of this type with the same key is already being tracked.", ex.Message);
        }
    }
}
