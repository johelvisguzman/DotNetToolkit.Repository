namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Options;
    using Data;
    using System;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class FileStreamRepositoryTests : TestBase
    {
        public FileStreamRepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            ForAllFileStreamContextProviders(TestThrowsIfDeleteWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            ForAllFileStreamContextProviders(TestThrowsIfUpdateWhenEntityNoInStore);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            ForAllFileStreamContextProviders(TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue);
        }

        [Fact]
        public void ThrowsIfBeginTransaction()
        {
            ForAllFileStreamContextProviders(TestThrowsIfBeginTransaction);
        }

        [Fact]
        public void ThrowsIfExecutingQuery()
        {
            ForAllFileStreamContextProviders(TestThrowsIfExecutingQuery);
        }

        private static void TestThrowsIfDeleteWhenEntityNoInStore(IRepositoryOptions options)
        {
            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the store.", ex.Message);
        }

        private static void TestThrowsIfUpdateWhenEntityNoInStore(IRepositoryOptions options)
        {
            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the store.", ex.Message);
        }

        private static void TestThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue(IRepositoryOptions options)
        {
            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the store because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        private static void TestThrowsIfBeginTransaction(IRepositoryOptions options)
        {
            var ex = Assert.Throws<NotSupportedException>(() => new UnitOfWork(options));
            Assert.Equal("This context provider does not support transactions.", ex.Message);
        }

        private static void TestThrowsIfExecutingQuery(IRepositoryOptions options)
        {
            var repo = new Repository<Customer>(options);

            var ex = Assert.Throws<NotSupportedException>(() => repo.ExecuteSqlCommand("SELECT * FROM Customers"));
            Assert.Equal("This context provider does not support SQL query execution.", ex.Message);
        }
    }
}
