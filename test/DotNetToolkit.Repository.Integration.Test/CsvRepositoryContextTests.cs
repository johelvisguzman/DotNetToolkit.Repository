namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using Csv;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class CsvRepositoryContextTests : TestBase
    {
        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<CsvRepositoryContextFactory>().ToList().ForEach(TestThrowsIfDeleteWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            GetRepositoryContextFactories().OfType<CsvRepositoryContextFactory>().ToList().ForEach(TestThrowsIfUpdateWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            GetRepositoryContextFactories().OfType<CsvRepositoryContextFactory>().ToList().ForEach(TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue);
        }

        private static void TestThrowsIfDeleteWhenEntityNoInStore(CsvRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the file.", ex.Message);
        }

        private static void TestThrowsIfUpdateWhenEntityNoInStore(CsvRepositoryContextFactory contextFactory)
        {
            var repo = new Repository<Customer>(contextFactory.Create());

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the file.", ex.Message);
        }

        private static void TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue(CsvRepositoryContextFactory contextFactory)
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
            var context = new CsvRepositoryContextFactory(Path.GetTempPath() + Guid.NewGuid().ToString("N"));

            var ex = Assert.Throws<NotSupportedException>(() => context.Create().BeginTransaction());
            Assert.Equal("The repository context does not support transactions.", ex.Message);
        }
    }
}
