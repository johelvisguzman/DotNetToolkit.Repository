namespace DotNetToolkit.Repository.Integration.Test.ContextProvider
{
    using Data;
    using InMemory;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
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
        public void CanValidateOnCrudOperations()
        {
            const string requiredNameError = "The Name field is required.";

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repo = new Repository<CustomerWithRequiredName>(options);
            var entity = new CustomerWithRequiredName();

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(entity));
            Assert.Equal(requiredNameError, ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entity));
            Assert.Equal(requiredNameError, ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repo.Delete(entity));
            Assert.Equal(requiredNameError, ex.Message);
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

        [Fact]
        public void CanFetchNestedNavPropertiesBidirectional()
        {
            var options = BuildOptions(ContextProviderType.InMemory);
            var factory = new RepositoryFactory(options);

            var repoA = factory.Create<TableA>();
            var a = new TableA { Id = 99 };

            repoA.Add(a);
            repoA.Add(new TableA { Id = 2333 });
            repoA.Add(new TableA { Id = 9 });

            var repoB = factory.Create<TableB>();
            var b = new TableB { Id = 10, TableAId = a.Id };

            repoB.Add(b);

            var repoC = factory.Create<TableC>();
            var c = new TableC { Id = 10, TableBId = b.Id };

            repoC.Add(c);

            var repoD = factory.Create<TableD>();
            var d = new TableD { Id = 500, TableCId = c.Id };

            repoD.Add(d);

            var queryOptions = new QueryOptions<TableA>()
                .WithFetch(x => x.TableB)
                .WithFetch(x => x.TableB.TableC)
                .WithFetch(x => x.TableB.TableC.TableD)
                .WithFilter(x => x.Id == a.Id);

            var result = repoA.Find(queryOptions);

            Assert.NotNull(result.TableB);
            Assert.NotNull(result.TableB.TableA);
            Assert.Equal(b.Id, result.TableB.Id);

            Assert.NotNull(result.TableB.TableC);
            Assert.NotNull(result.TableB.TableC.TableB);
            Assert.Equal(c.Id, result.TableB.TableC.Id);

            Assert.NotNull(result.TableB.TableC.TableD);
            Assert.NotNull(result.TableB.TableC.TableD.TableC);
            Assert.Equal(d.Id, result.TableB.TableC.TableD.Id);
        }

        [Fact]
        public void CanFetchCollectionsWithNestedNavPropertiesBidirectional()
        {
            var options = BuildOptions(ContextProviderType.InMemory);
            var factory = new RepositoryFactory(options);

            var repoA = factory.Create<TableA>();
            var a = new TableA { Id = 99 };

            repoA.Add(a);

            var repoB = factory.Create<TableB>();
            var b = new TableB { Id = 10, TableAId = a.Id };

            repoB.Add(b);

            var repoF = factory.Create<TableF>();
            var f = new TableF { Id = 23 };

            repoF.Add(f);

            var repoE = factory.Create<TableE>();

            var eList = new List<TableE>();

            for (int i = 0; i < 10; i++)
            {
                eList.Add(new TableE { Id = 1000 + i, TableAId = a.Id, TableFId = f.Id });
            }

            repoE.Add(eList);

            var queryOptions = new QueryOptions<TableA>()
                .WithFetch(x => x.TableB)
                .WithFetch(x => x.TableE)
                .WithFetch(x => x.TableE.Select(y => y.TableF))
                .WithFilter(x => x.Id == a.Id);

            var result = repoA.Find(queryOptions);

            Assert.NotNull(result.TableB);
            Assert.NotNull(result.TableB.TableA);
            Assert.Equal(b.Id, result.TableB.Id);

            Assert.NotEmpty(result.TableE);
            Assert.NotNull(result.TableE.First().TableF);
            Assert.NotNull(result.TableE.First().TableA);
            Assert.Equal(eList.Select(x => x.Id), result.TableE.Select(x => x.Id));
        }

        [Fact]
        public void CanFetchNavPropertiesOneDirection()
        {
            var options = BuildOptions(ContextProviderType.InMemory);
            var factory = new RepositoryFactory(options);

            var repoJ = factory.Create<TableJ>();
            var j = new TableJ { Id = 55 };

            repoJ.Add(j);

            var repoI = factory.Create<TableI>();
            var i = new TableI { Id = 33, TableJId = j.Id };

            repoI.Add(i);

            var repoH = factory.Create<TableH>();
            var h = new TableH { Id = 10, TableIId = i.Id };

            repoH.Add(h);

            var repoG = factory.Create<TableG>();
            var g = new TableG { Id = 99, TableHId = h.Id };

            repoG.Add(g);

            var queryOptions = new QueryOptions<TableG>()
                .WithFetch(x => x.TableH)
                .WithFetch(x => x.TableH.TableI)
                .WithFetch(x => x.TableH.TableI.TableJ)
                .WithFilter(x => x.Id == g.Id);

            var result = repoG.Find(queryOptions);

            Assert.NotNull(result.TableH);
            Assert.Equal(h.Id, result.TableH.Id);

            Assert.NotNull(result.TableH.TableI);
            Assert.Equal(i.Id, result.TableH.TableI.Id);

            Assert.NotNull(result.TableH.TableI.TableJ);
            Assert.Equal(j.Id, result.TableH.TableI.TableJ.Id);
        }

        class TableA
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public TableB TableB { get; set; }
            public ICollection<TableE> TableE { get; set; }
        }

        class TableB
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableAId { get; set; }
            public TableA TableA { get; set; }
            public TableC TableC { get; set; }
        }

        class TableC
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableBId { get; set; }
            public TableB TableB { get; set; }
            public TableD TableD { get; set; }
        }

        class TableD
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableCId { get; set; }
            public TableC TableC { get; set; }
        }

        class TableE
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableAId { get; set; }
            public TableA TableA { get; set; }
            public int TableFId { get; set; }
            public TableF TableF { get; set; }
        }

        class TableF
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public TableE TableE { get; set; }
        }

        class TableG
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableHId { get; set; }
            public TableH TableH { get; set; }
        }

        class TableH
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public TableG TableG { get; set; }
            public int TableIId { get; set; }
            public TableI TableI { get; set; }
        }

        class TableI
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int TableJId { get; set; }
            public TableJ TableJ { get; set; }
        }

        class TableJ
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
        }
    }
}
