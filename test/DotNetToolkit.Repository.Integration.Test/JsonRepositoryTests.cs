namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using System;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class JsonRepositoryContextTests : TestBase
    {
        public JsonRepositoryContextTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.Json));

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the store.", ex.Message);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.Json));

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the store.", ex.Message);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.Json));

            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the store because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        [Fact]
        public void ThrowsIfBeginTransaction()
        {
            var ex = Assert.Throws<NotSupportedException>(() => new UnitOfWork(BuildOptions(ContextProviderType.Json)));
            Assert.Equal("This context provider does not support transactions.", ex.Message);
        }

        [Fact]
        public void ThrowsIfExecutingQuery()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.Json));

            var ex = Assert.Throws<NotSupportedException>(() => repo.ExecuteSqlCommand("SELECT * FROM Customers"));
            Assert.Equal("This context provider does not support SQL query execution.", ex.Message);
        }
    }
}
