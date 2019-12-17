namespace DotNetToolkit.Repository.Integration.Test.ContextProvider
{
    using Configuration.Options;
    using Data;
    using InMemory;
    using System;
    using Transactions;
    using Xunit;
    using Xunit.Abstractions;

    public class InMemoryRepositoryTests : TestBase
    {
        public InMemoryRepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CanScoped()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo1 = new Repository<Customer>(options);
            var entity1 = new Customer { Name = "Random Name" };

            repo1.Add(entity1);

            Assert.NotNull(repo1.Find(x => x.Name.Equals("Random Name")));
            Assert.Equal(1, entity1.Id);

            var repo2 = new Repository<Customer>(options);
            var entity2 = new Customer { Name = "Random Name" };

            repo2.Add(entity2);

            Assert.Equal(2, entity2.Id);
            Assert.Equal(2, repo2.Count(x => x.Name.Equals("Random Name")));

            options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo3 = new Repository<Customer>(options);
            var entity3 = new Customer { Name = "Random Name" };

            repo3.Add(entity3);

            Assert.NotNull(repo3.Find(x => x.Name.Equals("Random Name")));
            Assert.Equal(1, entity3.Id);
        }

        [Fact]
        public void CanBeginNullTransactionWhenWarningIgnored()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(ignoreTransactionWarning: true)
                .Options;

            var uow = new UnitOfWork(options);
        }

        [Fact]
        public void ThrowsIfDeleteWhenEntityNoInStore()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        [Fact]
        public void ThrowsIfUpdateWhenEntityNoInStore()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the in-memory store.", ex.Message);
        }

        [Fact]
        public void ThrowsIfAddingEntityOfSameTypeWithSamePrimaryKeyValue()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<Customer>(options);

            var entity = new Customer { Name = "Random Name" };

            repo.Add(entity);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal($"The instance of entity type '{typeof(Customer)}' cannot be added to the in-memory store because another instance of this type with the same key is already being tracked.", ex.Message);
        }

        [Fact]
        public void ThrowsIfBeginTransaction()
        {
            var ex = Assert.Throws<NotSupportedException>(() => new UnitOfWork(BuildOptions(ContextProviderType.InMemory)));
            Assert.Equal("This context provider does not support transactions.", ex.Message);
        }

        [Fact]
        public void ThrowsIfExecutingQuery()
        {
            var repo = new Repository<Customer>(BuildOptions(ContextProviderType.InMemory));

            var ex = Assert.Throws<NotSupportedException>(() => repo.ExecuteSqlCommand("SELECT * FROM Customers"));
            Assert.Equal("This context provider does not support SQL query execution.", ex.Message);

            ex = Assert.Throws<NotSupportedException>(() => repo.ExecuteSqlQuery("SELECT * FROM Customers", r => new Customer()
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            }));
            Assert.Equal("This context provider does not support SQL query execution.", ex.Message);
        }

        [Fact]
        public void CanExecuteQueryWhenWarningIgnored()
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(ignoreTransactionWarning: false, ignoreSqlQueryWarning: true)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.ExecuteSqlCommand("SELECT * FROM Customers");
            repo.ExecuteSqlQuery("SELECT * FROM Customers", r => new Customer()
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            });
        }
    }
}
