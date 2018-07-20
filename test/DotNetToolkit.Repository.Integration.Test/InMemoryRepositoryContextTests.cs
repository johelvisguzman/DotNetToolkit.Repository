namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using InMemory;
    using System;
    using System.Linq;
    using Xunit;

    public class InMemoryRepositoryContextTests : TestBase
    {
        [Fact]
        public void CanScoped()
        {
            var databaseName = Guid.NewGuid().ToString();

            using (var context = new InMemoryRepositoryContextFactory(databaseName).Create())
            {
                var repo = new Repository<Customer>(context);
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }

            using (var context = new InMemoryRepositoryContextFactory(databaseName).Create())
            {
                var repo = new Repository<Customer>(context);
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.Equal(2, entity.Id);
                Assert.Equal(2, repo.Count(x => x.Name.Equals("Random Name")));
            }

            using (var context = new InMemoryRepositoryContextFactory(Guid.NewGuid().ToString()).Create())
            {
                var repo = new Repository<Customer>(context);
                var entity = new Customer { Name = "Random Name" };

                repo.Add(entity);

                Assert.NotNull(repo.Find(x => x.Name.Equals("Random Name")));
                Assert.Equal(1, entity.Id);
            }
        }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<InMemoryRepositoryContextFactory>().ToList().ForEach(TestThrowsIfDeleteWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<InMemoryRepositoryContextFactory>().ToList().ForEach(TestThrowsIfUpdateWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            GetRepositoryContextFactories().OfType<InMemoryRepositoryContextFactory>().ToList().ForEach(TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue);
        }

        private static void TestThrowsIfDeleteWhenEntityNoInStore(InMemoryRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        private static void TestThrowsIfUpdateWhenEntityNoInStore(InMemoryRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        private static void TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue(InMemoryRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the in-memory store because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        [Fact]
        public void ThrowsIfBeginTransaction()
        {
            var configuration = new InMemoryRepositoryContextFactory();

            var ex = Assert.Throws<NotSupportedException>(() => configuration.Create().BeginTransaction());
            Assert.Equal("The repository context does not support transactions.", ex.Message);
        }
    }
}
