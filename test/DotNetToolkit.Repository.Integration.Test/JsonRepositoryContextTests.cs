namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Json;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class JsonRepositoryContextTests : TestBase
    {
        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<JsonRepositoryContextFactory>().ToList().ForEach(TestThrowsIfDeleteWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<JsonRepositoryContextFactory>().ToList().ForEach(TestThrowsIfUpdateWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            GetRepositoryContextFactories().OfType<JsonRepositoryContextFactory>().ToList().ForEach(TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue);
        }

        private static void TestThrowsIfDeleteWhenEntityNoInStore(JsonRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the file.", ex.Message);
        }

        private static void TestThrowsIfUpdateWhenEntityNoInStore(JsonRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the file.", ex.Message);
        }

        private static void TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue(JsonRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the file because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        [Fact]
        public void ThrowsIfBeginTransaction()
        {
            var context = new JsonRepositoryContextFactory(Path.GetTempPath() + Guid.NewGuid().ToString("N"));

            var ex = Assert.Throws<NotSupportedException>(() => context.Create().BeginTransaction());
            Assert.Equal("The repository context does not support transactions.", ex.Message);
        }
    }
}
