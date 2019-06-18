namespace DotNetToolkit.Repository.Integration.Test
{
    using AdoNet;
    using AdoNet.Internal;
    using AdoNet.Internal.Schema;
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Configuration.Options;
    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class AdoNetRepositoryTests : TestBase
    {
        public AdoNetRepositoryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CanUseExistingConnection()
        {
            var repo = new Repository<Data.Customer>(BuildOptions(ContextProviderType.AdoNet));

            repo.Add(new Data.Customer());

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void DeleteWithKeyDataAttribute()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<CustomerWithKeyAnnotation>();

            const string name = "Random Name";

            var entity = new CustomerWithKeyAnnotation { Name = name };

            repo.Add(entity);

            Assert.True(repo.Exists(entity.Key));

            repo.Delete(entity.Key);

            Assert.False(repo.Exists(entity.Key));
        }

        [Fact]
        public async Task DeleteWithKeyDataAttributeAsync()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<CustomerWithKeyAnnotation>();

            const string name = "Random Name";

            var entity = new CustomerWithKeyAnnotation { Name = name };

            await repo.AddAsync(entity);

            Assert.True(await repo.ExistsAsync(entity.Key));

            await repo.DeleteAsync(entity.Key);

            Assert.False(await repo.ExistsAsync(entity.Key));
        }

        [Fact]
        public void FindWithComplexExpressions()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<Customer>();

            var numOneVar = 1;
            var numTwoVar = 2;
            var numThreeVar = 3;

            const int numOneConst = 1;
            const int numTwoConst = 2;
            const int numThreeConst = 3;

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            repo.Add(entities);

            var customer = entities[0];

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => 1 == x.Id).Id);
            Assert.Equal(1, repo.Find(x => x.Id == 1).Id);
            Assert.Equal(1, repo.Find(x => 1 == 1).Id);
            Assert.Null(repo.Find(x => 1 == 2));
            Assert.Equal(1, repo.Find(x => x.Id == x.Id).Id);
            Assert.Equal(1, repo.Find(x => numOneVar == x.Id).Id);
            Assert.Equal(1, repo.Find(x => x.Id == numOneVar).Id);
            Assert.Equal(1, repo.Find(x => numOneVar == numOneVar).Id);
            Assert.Equal(1, repo.Find(x => numOneConst == x.Id).Id);
            Assert.Equal(1, repo.Find(x => x.Id == numOneConst).Id);
            Assert.Equal(1, repo.Find(x => numOneConst == numOneConst).Id);
            Assert.Equal(1, repo.Find(x => customer.Id == x.Id).Id);
            Assert.Equal(1, repo.Find(x => x.Id == customer.Id).Id);
            Assert.Equal(1, repo.Find(x => customer.Id == customer.Id).Id);
            Assert.Equal(1, repo.Find<int>(x => numOneConst == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => 1 == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => 1 == 1, x => x.Id));
            Assert.Equal(0, repo.Find<int>(x => 1 == 2, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => numOneVar == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == numOneVar, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == numOneConst, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => customer.Id == x.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == customer.Id, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => customer.Id == customer.Id, x => x.Id));

            // boolean
            Assert.Equal(1, repo.Find(x => true).Id);
            Assert.Null(repo.Find(x => false));
            Assert.Equal(1, repo.Find(x => true, x => x.Id));
            Assert.Equal(0, repo.Find(x => false, x => x.Id));

            // method calls
            Assert.Equal(1, repo.Find(x => x.Name.StartsWith("Random")).Id);
            Assert.Equal(2, repo.Find(x => x.Name.Equals("Random Name 2")).Id);
            Assert.Equal(3, repo.Find(x => x.Name.Contains("Test")).Id);
            Assert.Equal(4, repo.Find(x => x.Name.EndsWith("4")).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().StartsWith(numOneVar.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().Equals(numOneVar.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().Contains(numOneVar.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().EndsWith(numOneVar.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().StartsWith(numOneConst.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().Equals(numOneConst.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().Contains(numOneConst.ToString())).Id);
            Assert.Equal(1, repo.Find(x => x.Id.ToString().EndsWith(numOneConst.ToString())).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().StartsWith(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Name.StartsWith("Random"), x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Equal(4, repo.Find<int>(x => x.Name.EndsWith("4"), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().StartsWith(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().Equals(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().Contains(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().EndsWith(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().Equals(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().Contains(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id.ToString().EndsWith(numOneConst.ToString()), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => x.Id == 1).Id);
            Assert.Equal(1, repo.Find(x => x.Id != 2).Id);
            Assert.Equal(2, repo.Find(x => x.Id > 1).Id);
            Assert.Equal(3, repo.Find(x => x.Id >= 3).Id);
            Assert.Equal(1, repo.Find(x => x.Id < 2).Id);
            Assert.Equal(1, repo.Find(x => x.Id <= 2).Id);
            Assert.Equal(1, repo.Find(x => x.Id == numOneVar).Id);
            Assert.Equal(1, repo.Find(x => x.Id != numTwoVar).Id);
            Assert.Equal(2, repo.Find(x => x.Id > numOneVar).Id);
            Assert.Equal(3, repo.Find(x => x.Id >= numThreeVar).Id);
            Assert.Equal(1, repo.Find(x => x.Id < numTwoVar).Id);
            Assert.Equal(1, repo.Find(x => x.Id <= numTwoVar).Id);
            Assert.Equal(1, repo.Find(x => x.Id == numOneConst).Id);
            Assert.Equal(1, repo.Find(x => x.Id != numTwoConst).Id);
            Assert.Equal(2, repo.Find(x => x.Id > numOneConst).Id);
            Assert.Equal(3, repo.Find(x => x.Id >= numThreeConst).Id);
            Assert.Equal(1, repo.Find(x => x.Id < numTwoConst).Id);
            Assert.Equal(1, repo.Find(x => x.Id <= numTwoConst).Id);
            Assert.Equal(1, repo.Find<int>(x => x.Id == numOneConst, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id != 2, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1, x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Id >= 3, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id <= 2, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id == numOneVar, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id != numTwoVar, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > numOneVar, x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Id >= numThreeVar, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id < numTwoVar, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id <= numTwoVar, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id != numTwoConst, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > numOneConst, x => x.Id));
            Assert.Equal(3, repo.Find<int>(x => x.Id >= numThreeConst, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id < numTwoConst, x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => x.Id <= numTwoConst, x => x.Id));

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(1, repo.Find(x => (x.Id == 1) || (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) || (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneVar) || (x.Id >= numOneVar)).Id);
            Assert.Equal(1, repo.Find(x => (numOneVar == x.Id) || (numOneVar <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneConst) || (x.Id >= numOneConst)).Id);
            Assert.Equal(1, repo.Find(x => (numOneConst == x.Id) || (numOneConst <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneConst) || (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneVar) || (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneVar == x.Id) || (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneConst == x.Id) || (numOneConst <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => (x.Id == 1) | (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) | (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneVar) | (x.Id >= numOneVar)).Id);
            Assert.Equal(1, repo.Find(x => (numOneVar == x.Id) | (numOneVar <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneConst) | (x.Id >= numOneConst)).Id);
            Assert.Equal(1, repo.Find(x => (numOneConst == x.Id) | (numOneConst <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneConst) | (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneVar) | (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneVar == x.Id) | (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneConst == x.Id) | (numOneConst <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, repo.Find(x => x.Id > 1 && x.Id < 3).Id);
            Assert.Equal(2, repo.Find(x => (x.Id > 1) && (x.Id < 3)).Id);
            Assert.Equal(2, repo.Find(x => x.Id > numOneVar && x.Id < numThreeVar).Id);
            Assert.Equal(2, repo.Find(x => x.Id > numOneConst && x.Id < numThreeConst).Id);
            Assert.Equal(2, repo.Find(x => (x.Id > numOneConst) && (x.Id < numThreeConst)).Id);
            Assert.Equal(2, repo.Find(x => (x.Id > numOneVar) && (x.Id < numThreeVar)).Id);
            Assert.Equal(2, repo.Find<int>(x => x.Id > numOneConst && x.Id < numThreeConst, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => x.Id > numOneVar && x.Id < numThreeVar, x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => (x.Id > numOneVar) && (x.Id < numThreeVar), x => x.Id));
            Assert.Equal(2, repo.Find<int>(x => (x.Id > numOneConst) && (x.Id < numThreeConst), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.Find(x => (x.Id == 1) & (x.Id >= 1)).Id);
            Assert.Equal(1, repo.Find(x => (1 == x.Id) & (1 <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneVar) | (x.Id >= numOneVar)).Id);
            Assert.Equal(1, repo.Find(x => (numOneVar == x.Id) | (numOneVar <= x.Id)).Id);
            Assert.Equal(1, repo.Find(x => (x.Id == numOneConst) | (x.Id >= numOneConst)).Id);
            Assert.Equal(1, repo.Find(x => (numOneConst == x.Id) | (numOneConst <= x.Id)).Id);
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneConst) | (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (x.Id == numOneVar) | (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneVar == x.Id) | (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, repo.Find<int>(x => (numOneConst == x.Id) | (numOneConst <= x.Id), x => x.Id));
        }

        [Fact]
        public async void FindWithComplexExpressionsAsync()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<Customer>();

            var numOneVar = 1;
            var numTwoVar = 2;
            var numThreeVar = 3;

            const int numOneConst = 1;
            const int numTwoConst = 2;
            const int numThreeConst = 3;

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            await repo.AddAsync(entities);

            var customer = entities[0];

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => 1 == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == 1)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => 1 == 1)).Id);
            Assert.Null(await repo.FindAsync(x => 1 == 2));
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => numOneVar == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == numOneVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => numOneVar == numOneVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => numOneConst == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == numOneConst)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => numOneConst == numOneConst)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => customer.Id == x.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == customer.Id)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => customer.Id == customer.Id)).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => numOneConst == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => 1 == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => 1 == 1, x => x.Id));
            Assert.Equal(0, await repo.FindAsync<int>(x => 1 == 2, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => 1 == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => numOneVar == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == numOneVar, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == numOneConst, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => customer.Id == x.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == customer.Id, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => customer.Id == customer.Id, x => x.Id));

            // boolean
            Assert.Equal(1, (await repo.FindAsync(x => true)).Id);
            Assert.Null(await repo.FindAsync(x => false));
            Assert.Equal(1, await repo.FindAsync(x => true, x => x.Id));
            Assert.Equal(0, await repo.FindAsync(x => false, x => x.Id));

            // method calls
            Assert.Equal(1, (await repo.FindAsync(x => x.Name.StartsWith("Random"))).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Name.Equals("Random Name 2"))).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Name.Contains("Test"))).Id);
            Assert.Equal(4, (await repo.FindAsync(x => x.Name.EndsWith("4"))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().StartsWith(numOneVar.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().Equals(numOneVar.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().Contains(numOneVar.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().EndsWith(numOneVar.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().StartsWith(numOneConst.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().Equals(numOneConst.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().Contains(numOneConst.ToString()))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id.ToString().EndsWith(numOneConst.ToString()))).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Name.StartsWith("Random"), x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Equal(4, await repo.FindAsync<int>(x => x.Name.EndsWith("4"), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Name.StartsWith("Random"), x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Equal(4, await repo.FindAsync<int>(x => x.Name.EndsWith("4"), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().StartsWith(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().Equals(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().Contains(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().EndsWith(numOneVar.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().StartsWith(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().Equals(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().Contains(numOneConst.ToString()), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id.ToString().EndsWith(numOneConst.ToString()), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == 1)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id != 2)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > 1)).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Id >= 3)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id < 2)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id <= 2)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == numOneVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id != numTwoVar)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > numOneVar)).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Id >= numThreeVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id < numTwoVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id <= numTwoVar)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id == numOneConst)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id != numTwoConst)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > numOneConst)).Id);
            Assert.Equal(3, (await repo.FindAsync(x => x.Id >= numThreeConst)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id < numTwoConst)).Id);
            Assert.Equal(1, (await repo.FindAsync(x => x.Id <= numTwoConst)).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == numOneConst, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id != 2, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > 1, x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Id >= 3, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id <= 2, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id != 2, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > 1, x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Id >= 3, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id <= 2, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id == numOneVar, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id != numTwoVar, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > numOneVar, x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Id >= numThreeVar, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id < numTwoVar, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id <= numTwoVar, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id != numTwoConst, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > numOneConst, x => x.Id));
            Assert.Equal(3, await repo.FindAsync<int>(x => x.Id >= numThreeConst, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id < numTwoConst, x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => x.Id <= numTwoConst, x => x.Id));

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) || (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) || (1 <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneVar) || (x.Id >= numOneVar))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneVar == x.Id) || (numOneVar <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneConst) || (x.Id >= numOneConst))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneConst == x.Id) || (numOneConst <= x.Id))).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneConst) || (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) || (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) || (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneVar) || (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneVar == x.Id) || (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneConst == x.Id) || (numOneConst <= x.Id), x => x.Id));

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) | (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) | (1 <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneVar) | (x.Id >= numOneVar))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneVar == x.Id) | (numOneVar <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneConst) | (x.Id >= numOneConst))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneConst == x.Id) | (numOneConst <= x.Id))).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneConst) | (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) | (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) | (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneVar) | (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneVar == x.Id) | (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneConst == x.Id) | (numOneConst <= x.Id), x => x.Id));

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > 1 && x.Id < 3)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => (x.Id > 1) && (x.Id < 3))).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > numOneVar && x.Id < numThreeVar)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => (x.Id > numOneVar) && (x.Id < numThreeVar))).Id);
            Assert.Equal(2, (await repo.FindAsync(x => x.Id > numOneConst && x.Id < numThreeConst)).Id);
            Assert.Equal(2, (await repo.FindAsync(x => (x.Id > numOneConst) && (x.Id < numThreeConst))).Id);
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > numOneConst && x.Id < numThreeConst, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => x.Id > numOneVar && x.Id < numThreeVar, x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => (x.Id > numOneVar) && (x.Id < numThreeVar), x => x.Id));
            Assert.Equal(2, await repo.FindAsync<int>(x => (x.Id > numOneConst) && (x.Id < numThreeConst), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == 1) & (x.Id >= 1))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (1 == x.Id) & (1 <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneVar) & (x.Id >= numOneVar))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneVar == x.Id) & (numOneVar <= x.Id))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (x.Id == numOneConst) & (x.Id >= numOneConst))).Id);
            Assert.Equal(1, (await repo.FindAsync(x => (numOneConst == x.Id) & (numOneConst <= x.Id))).Id);
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneConst) & (x.Id >= numOneConst), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (x.Id == numOneVar) & (x.Id >= numOneVar), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneVar == x.Id) & (numOneVar <= x.Id), x => x.Id));
            Assert.Equal(1, await repo.FindAsync<int>(x => (numOneConst == x.Id) & (numOneConst <= x.Id), x => x.Id));
        }

        [Fact]
        public void FindAllWithComplexExpressions()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            repo.Add(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, repo.FindAll(x => 1 == x.Id).First().Id);
            Assert.Equal(1, repo.FindAll(x => x.Id == 1).First().Id);
            Assert.Equal(1, repo.FindAll<int>(x => 1 == x.Id, x => x.Id).First());
            Assert.Equal(1, repo.FindAll<int>(x => x.Id == 1, x => x.Id).First());

            // boolean
            Assert.Equal(1, repo.FindAll(x => true).First().Id);
            Assert.Empty(repo.FindAll(x => false));
            Assert.Equal(1, repo.FindAll(x => true, x => x.Id).First());
            Assert.Empty(repo.FindAll(x => false, x => x.Id));

            // method calls
            Assert.Equal(4, repo.FindAll(x => x.Name.StartsWith("Random")).Count());
            Assert.Single(repo.FindAll(x => x.Name.Equals("Random Name 2")));
            Assert.Single(repo.FindAll(x => x.Name.Contains("Test")));
            Assert.Single(repo.FindAll(x => x.Name.EndsWith("4")));
            Assert.Equal(4, repo.FindAll<int>(x => x.Name.StartsWith("Random"), x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Single(repo.FindAll(x => x.Id == 1));
            Assert.Equal(3, repo.FindAll(x => x.Id != 2).Count());
            Assert.Equal(3, repo.FindAll(x => x.Id > 1).Count());
            Assert.Equal(2, repo.FindAll(x => x.Id >= 3).Count());
            Assert.Single(repo.FindAll(x => x.Id < 2));
            Assert.Equal(2, repo.FindAll(x => x.Id <= 2).Count());
            Assert.Single(repo.FindAll<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(3, repo.FindAll<int>(x => x.Id != 2, x => x.Id).Count());
            Assert.Equal(3, repo.FindAll<int>(x => x.Id > 1, x => x.Id).Count());
            Assert.Equal(2, repo.FindAll<int>(x => x.Id >= 3, x => x.Id).Count());
            Assert.Single(repo.FindAll<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(2, repo.FindAll<int>(x => x.Id <= 2, x => x.Id).Count());

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, repo.FindAll(x => (x.Id == 1) || (x.Id == 4)).Count());
            Assert.Equal(2, repo.FindAll(x => (1 == x.Id) || (4 == x.Id)).Count());
            Assert.Equal(2, repo.FindAll<int>(x => (x.Id == 1) || (x.Id == 4), x => x.Id).Count());
            Assert.Equal(2, repo.FindAll<int>(x => (1 == x.Id) || (4 == x.Id), x => x.Id).Count());

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(2, repo.FindAll(x => (x.Id == 1) | (x.Id == 4)).Count());
            Assert.Equal(2, repo.FindAll(x => (1 == x.Id) | (4 == x.Id)).Count());
            Assert.Equal(2, repo.FindAll<int>(x => (x.Id == 1) | (x.Id == 4), x => x.Id).Count());
            Assert.Equal(2, repo.FindAll<int>(x => (1 == x.Id) | (4 == x.Id), x => x.Id).Count());

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Single(repo.FindAll(x => x.Id > 1 && x.Id < 3));
            Assert.Single(repo.FindAll(x => (x.Id > 1) && (x.Id < 3)));
            Assert.Single(repo.FindAll<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Single(repo.FindAll<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Single(repo.FindAll(x => (x.Id == 1) & (x.Id >= 1)));
            Assert.Single(repo.FindAll(x => (1 == x.Id) & (1 <= x.Id)));
            Assert.Single(repo.FindAll<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Single(repo.FindAll<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public async void FindAllWithComplexExpressionsAsync()
        {
            var repoFactory = new RepositoryFactory(BuildOptions(ContextProviderType.AdoNet));

            var repo = repoFactory.Create<Customer>();

            var entities = new List<Customer>
            {
                new Customer { Id = 1, Name = "Random Name 1" },
                new Customer { Id = 2, Name = "Random Name 2" },
                new Customer { Id = 3, Name = "Random Test Name 3" },
                new Customer { Id = 4, Name = "Random Name 4" }
            };

            await repo.AddAsync(entities);

            // The ado.net repository should be able translate the expression into a valid sql query string to execute
            // things like parentheses and operators should be tested

            // property variables on the left and constants on the right (and vice verse)
            Assert.Equal(1, (await repo.FindAllAsync(x => 1 == x.Id)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync(x => x.Id == 1)).First().Id);
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => 1 == x.Id, x => x.Id)).First());
            Assert.Equal(1, (await repo.FindAllAsync<int>(x => x.Id == 1, x => x.Id)).First());

            // boolean
            Assert.Equal(1, (await repo.FindAllAsync(x => true)).First().Id);
            Assert.Empty(await repo.FindAllAsync(x => false));
            Assert.Equal(1, (await repo.FindAllAsync(x => true, x => x.Id)).First());
            Assert.Empty(await repo.FindAllAsync(x => false, x => x.Id));

            // method calls
            Assert.Equal(4, (await repo.FindAllAsync(x => x.Name.StartsWith("Random"))).Count());
            Assert.Single(await repo.FindAllAsync(x => x.Name.Equals("Random Name 2")));
            Assert.Single(await repo.FindAllAsync(x => x.Name.Contains("Test")));
            Assert.Single(await repo.FindAllAsync(x => x.Name.EndsWith("4")));
            Assert.Equal(4, (await repo.FindAllAsync<int>(x => x.Name.StartsWith("Random"), x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.Equals("Random Name 2"), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.Contains("Test"), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Name.EndsWith("4"), x => x.Id));

            // relational and equality operators - property variables on the left and constants on the right (and vice verse)
            Assert.Single(await repo.FindAllAsync(x => x.Id == 1));
            Assert.Equal(3, (await repo.FindAllAsync(x => x.Id != 2)).Count());
            Assert.Equal(3, (await repo.FindAllAsync(x => x.Id > 1)).Count());
            Assert.Equal(2, (await repo.FindAllAsync(x => x.Id >= 3)).Count());
            Assert.Single(await repo.FindAllAsync(x => x.Id < 2));
            Assert.Equal(2, (await repo.FindAllAsync(x => x.Id <= 2)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id == 1, x => x.Id));
            Assert.Equal(3, (await repo.FindAllAsync<int>(x => x.Id != 2, x => x.Id)).Count());
            Assert.Equal(3, (await repo.FindAllAsync<int>(x => x.Id > 1, x => x.Id)).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => x.Id >= 3, x => x.Id)).Count());
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id < 2, x => x.Id));
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => x.Id <= 2, x => x.Id)).Count());

            // conditional or operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Equal(2, (await repo.FindAllAsync(x => (x.Id == 1) || (x.Id == 4))).Count());
            Assert.Equal(2, (await repo.FindAllAsync(x => (1 == x.Id) || (4 == x.Id))).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => (x.Id == 1) || (x.Id == 4), x => x.Id)).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => (1 == x.Id) || (4 == x.Id), x => x.Id)).Count());

            // logical or operations - property variables on the left and constants on the right (and vice verse)
            Assert.Equal(2, (await repo.FindAllAsync(x => (x.Id == 1) | (x.Id == 4))).Count());
            Assert.Equal(2, (await repo.FindAllAsync(x => (1 == x.Id) | (4 == x.Id))).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => (x.Id == 1) | (x.Id == 4), x => x.Id)).Count());
            Assert.Equal(2, (await repo.FindAllAsync<int>(x => (1 == x.Id) | (4 == x.Id), x => x.Id)).Count());

            // conditional and operations - property variables on the left and constants on the right (and vice verse)
            // relational and equality operators
            Assert.Single(await repo.FindAllAsync(x => x.Id > 1 && x.Id < 3));
            Assert.Single(await repo.FindAllAsync(x => (x.Id > 1) && (x.Id < 3)));
            Assert.Single(await repo.FindAllAsync<int>(x => x.Id > 1 && x.Id < 3, x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => (x.Id > 1) && (x.Id < 3), x => x.Id));

            // logical and operations - property variables on the left and constants on the right (and vice verse)
            Assert.Single(await repo.FindAllAsync(x => (x.Id == 1) & (x.Id >= 1)));
            Assert.Single(await repo.FindAllAsync(x => (1 == x.Id) & (1 <= x.Id)));
            Assert.Single(await repo.FindAllAsync<int>(x => (x.Id == 1) & (x.Id >= 1), x => x.Id));
            Assert.Single(await repo.FindAllAsync<int>(x => (1 == x.Id) & (1 <= x.Id), x => x.Id));
        }

        [Fact]
        public void ThrowsIfSchemaTableColumnsMismatchOnSaveChanges()
        {
            var connection = TestDbConnectionHelper.CreateConnection();
            var options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated: true)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            var repoFactory = new RepositoryFactory(options);

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = @"CREATE TABLE CustomersColumnNameMismatch (
                                        Id int IDENTITY PRIMARY KEY,
                                        Name nvarchar (100))";

                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = @"CREATE TABLE CustomersColumnNameMissing (
                                        Id int IDENTITY PRIMARY KEY,
                                        Name nvarchar (100))";

                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = @"CREATE TABLE CustomersKeyMismatch (
                                        Id int,
                                        Id1 int IDENTITY PRIMARY KEY)";

                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = @"CREATE TABLE CustomersColumnRequiredMissing (
                                        Id int IDENTITY PRIMARY KEY,
                                        Name nvarchar (100) NOT NULL)";

                command.ExecuteNonQuery();
            }

            var ex = Assert.Throws<InvalidOperationException>(() => repoFactory.Create<CustomerColumnNameMismatch>().Add(new CustomerColumnNameMismatch()));
            Assert.Equal($"The model '{typeof(CustomerColumnNameMismatch).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repoFactory.Create<CustomerColumnNameMissing>().Add(new CustomerColumnNameMissing()));
            Assert.Equal($"The model '{typeof(CustomerColumnNameMissing).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repoFactory.Create<CustomerKeyMismatch>().Add(new CustomerKeyMismatch()));
            Assert.Equal($"The model '{typeof(CustomerKeyMismatch).Name}' has changed since the database was created. Consider updating the database.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => repoFactory.Create<CustomerColumnRequiredMissing>().Add(new CustomerColumnRequiredMissing()));
            Assert.Equal($"The model '{typeof(CustomerColumnRequiredMissing).Name}' has changed since the database was created. Consider updating the database.", ex.Message);
        }

        [Fact]
        public void ThrowsIfThrowsIfSchemaTableForeignKeyAttributeOnPropertyNotFoundOnDependentType()
        {
            var options = BuildOptions(ContextProviderType.AdoNet);

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerNotCreatedWithForeignKeyAttributeNotFoundOnDependentType>(options).Add(new CustomerNotCreatedWithForeignKeyAttributeNotFoundOnDependentType()));
            Assert.Equal($"The ForeignKeyAttribute on property 'Address' on type '{typeof(CustomerNotCreatedWithForeignKeyAttributeNotFoundOnDependentType).FullName}' is not valid. The foreign key name 'AddressId' was not found on the dependent type '{typeof(CustomerNotCreatedWithForeignKeyAttributeNotFoundOnDependentType).FullName}'. The Name value should be a comma separated list of foreign key property names.", ex.Message);
        }

        [Fact]
        public void CreateTableOnSaveChanges()
        {
            var conventions = RepositoryConventions.Default<AdoNetRepositoryContext>();
            var ensureDatabaseCreated = true;
            var connection = TestDbConnectionHelper.CreateConnection();
            var schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            var options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;
            var classARepo = new Repository<ClassA>(options);

            Assert.False(schemaHelper.ExecuteTableExists<ClassB>());
            Assert.False(schemaHelper.ExecuteTableExists<ClassC>());
            Assert.Equal(0, classARepo.Count());

            classARepo.Add(new ClassA());

            Assert.True(schemaHelper.ExecuteTableExists<ClassB>());
            Assert.True(schemaHelper.ExecuteTableExists<ClassC>());
            Assert.Equal(1, classARepo.Count());

            //
            connection = TestDbConnectionHelper.CreateConnection();
            schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.False(schemaHelper.ExecuteTableExists<ClassA>());
            Assert.False(schemaHelper.ExecuteTableExists<ClassC>());

            var ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassB>(options).Add(new ClassB()));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassAs ]", ex.Message);

            Assert.True(schemaHelper.ExecuteTableExists<ClassA>());
            Assert.True(schemaHelper.ExecuteTableExists<ClassC>());

            new Repository<ClassA>(options).Add(new ClassA { Id = 1 });

            var classBRepo = new Repository<ClassB>(options);

            Assert.Equal(0, classBRepo.Count());

            classBRepo.Add(new ClassB() { ClassAId = 1 });

            Assert.Equal(1, classBRepo.Count());

            //
            connection = TestDbConnectionHelper.CreateConnection();
            schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.False(schemaHelper.ExecuteTableExists<ClassA>());
            Assert.False(schemaHelper.ExecuteTableExists<ClassB>());

            ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassC>(options).Add(new ClassC()));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassBs ]", ex.Message);

            Assert.True(schemaHelper.ExecuteTableExists<ClassA>());
            Assert.True(schemaHelper.ExecuteTableExists<ClassB>());

            ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassB>(options).Add(new ClassB { Id = 1 }));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassAs ]", ex.Message);

            new Repository<ClassA>(options).Add(new ClassA { Id = 1 });
            new Repository<ClassB>(options).Add(new ClassB { Id = 1, ClassAId = 1 });

            var classCRepo = new Repository<ClassC>(options);

            Assert.Equal(0, classCRepo.Count());

            classCRepo.Add(new ClassC() { ClassBId = 1 });

            Assert.Equal(1, classCRepo.Count());
        }

        [Fact]
        public async Task CreateTableOnSaveChangesAsync()
        {
            var conventions = RepositoryConventions.Default<AdoNetRepositoryContext>();
            var ensureDatabaseCreated = true;
            var connection = TestDbConnectionHelper.CreateConnection();
            var schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            var options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;
            var classARepo = new Repository<ClassA>(options);

            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassB>());
            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassC>());
            Assert.Equal(0, await classARepo.CountAsync());

            await classARepo.AddAsync(new ClassA());

            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassB>());
            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassC>());
            Assert.Equal(1, await classARepo.CountAsync());

            //
            connection = TestDbConnectionHelper.CreateConnection();
            schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassA>());
            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassC>());

            var ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassB>(options).Add(new ClassB()));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassAs ]", ex.Message);

            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassA>());
            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassC>());

            new Repository<ClassA>(options).Add(new ClassA { Id = 1 });

            var classBRepo = new Repository<ClassB>(options);

            Assert.Equal(0, await classBRepo.CountAsync());

            classBRepo.Add(new ClassB() { ClassAId = 1 });

            Assert.Equal(1, await classBRepo.CountAsync());

            //
            connection = TestDbConnectionHelper.CreateConnection();
            schemaHelper = new SchemaTableConfigurationHelper(new DbHelper(conventions, connection));
            options = new RepositoryOptionsBuilder()
                .UseAdoNet(connection, ensureDatabaseCreated)
                .UseConventions(c => c = conventions)
                .UseLoggerProvider(TestXUnitLoggerProvider)
                .Options;

            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassA>());
            Assert.False(await schemaHelper.ExecuteTableExistsAsync<ClassB>());

            ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassC>(options).Add(new ClassC()));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassBs ]", ex.Message);

            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassA>());
            Assert.True(await schemaHelper.ExecuteTableExistsAsync<ClassB>());

            ex = Assert.Throws<System.Data.SqlServerCe.SqlCeException>(() => new Repository<ClassB>(options).Add(new ClassB { Id = 1 }));
            Assert.Equal("A foreign key value cannot be inserted because a corresponding primary key value does not exist. [ Foreign key constraint name = FK_ClassAs ]", ex.Message);

            new Repository<ClassA>(options).Add(new ClassA { Id = 1 });
            new Repository<ClassB>(options).Add(new ClassB { Id = 1, ClassAId = 1 });

            var classCRepo = new Repository<ClassC>(options);

            Assert.Equal(0, await classCRepo.CountAsync());

            await classCRepo.AddAsync(new ClassC() { ClassBId = 1 });

            Assert.Equal(1, await classCRepo.CountAsync());
        }

        [Fact]
        public void ThrowsIfUnableToDeterminePrincipalOnSaveChanges()
        {
            var options = BuildOptions(ContextProviderType.AdoNet);

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<ClassD>(options).Add(new ClassD()));
            Assert.Equal($"Unable to determine the principal end of an association between the types '{typeof(ClassE).FullName}' and '{typeof(ClassD).FullName}'. The principal end of this association must be explicitly configured using data annotations.", ex.Message);

            options = BuildOptions(ContextProviderType.AdoNet);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<ClassE>(options).Add(new ClassE()));
            Assert.Equal($"Unable to determine the principal end of an association between the types '{typeof(ClassD).FullName}' and '{typeof(ClassE).FullName}'. The principal end of this association must be explicitly configured using data annotations.", ex.Message);

            options = BuildOptions(ContextProviderType.AdoNet);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<ClassF>(options).Add(new ClassF()));
            Assert.Equal($"Unable to determine the principal end of an association between the types '{typeof(ClassG).FullName}' and '{typeof(ClassF).FullName}'. The principal end of this association must be explicitly configured using data annotations.", ex.Message);

            options = BuildOptions(ContextProviderType.AdoNet);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<ClassG>(options).Add(new ClassG()));
            Assert.Equal($"Unable to determine the principal end of an association between the types '{typeof(ClassF).FullName}' and '{typeof(ClassG).FullName}'. The principal end of this association must be explicitly configured using data annotations.", ex.Message);
        }

        [Fact]
        public void ThrowsIfUnableToDetermineCompositePrimaryKeyOrderingOnSaveChanges()
        {
            var options = BuildOptions(ContextProviderType.AdoNet);

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerAddressWithTwoCompositePrimaryKeyAndNoOrdering>(options).Add(new CustomerAddressWithTwoCompositePrimaryKeyAndNoOrdering()));
            Assert.Equal(string.Format(AdoNet.Properties.Resources.UnableToDetermineCompositePrimaryKeyOrdering, "primary", typeof(CustomerAddressWithTwoCompositePrimaryKeyAndNoOrdering).FullName), ex.Message);
        }

        [Fact]
        public void ThrowsIfUnableToDetermineCompositeForeignKeyOrderingOnSaveChanges()
        {
            var options = BuildOptions(ContextProviderType.AdoNet);

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<CustomerWithTwoCompositeForeignKeyAndNoOrdering>(options).Add(new CustomerWithTwoCompositeForeignKeyAndNoOrdering()));
            Assert.Equal(string.Format(AdoNet.Properties.Resources.UnableToDetermineCompositePrimaryKeyOrdering, "foreign", typeof(CustomerWithTwoCompositeForeignKeyAndNoOrdering).FullName), ex.Message);
        }

        [Table("Customers")]
        class CustomerWithKeyAnnotation
        {
            [Key]
            [Column("Id")]
            public int Key { get; set; }
            public string Name { get; set; }
            public CustomerAddress Address { get; set; }
        }

        class CustomerAddressWithTwoCompositePrimaryKeyAndNoOrdering
        {
            [Key]
            public int Id1 { get; set; }
            [Key]
            public int Id2 { get; set; }
            public string Street1 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
        }

        class CustomerWithTwoCompositeForeignKeyAndNoOrdering
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [ForeignKey("Address")]
            public int AddressId1 { get; set; }
            [ForeignKey("Address")]
            public int AddressId2 { get; set; }
            public CustomerAddressWithTwoCompositePrimaryKeyAndNoOrdering Address { get; set; }
        }

        [Table("CustomersColumnNameMismatch")]
        class CustomerColumnNameMismatch
        {
            public int Id { get; set; }
            public string MismatchName { get; set; }
        }

        [Table("CustomersColumnNameMissing")]
        class CustomerColumnNameMissing
        {
            public int Id { get; set; }
        }

        [Table("CustomersColumnNameMissing")]
        class CustomerKeyMismatch
        {
            public int Id { get; set; }
            [Key]
            public int Id1 { get; set; }
        }

        [Table("CustomersColumnRequiredMissing")]
        class CustomerColumnRequiredMissing
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Table("CustomersNotCreatedWithForeignKeyAttributeNotFoundOnDependentType")]
        class CustomerNotCreatedWithForeignKeyAttributeNotFoundOnDependentType
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [ForeignKey("AddressId")]
            public CustomerAddress Address { get; set; }
        }

        class ClassA
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public string NameA { get; set; }
            public ClassB ClassB { get; set; }
        }

        class ClassB
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassAId { get; set; }
            public int ClassCId { get; set; }
            [Required]
            public ClassA ClassA { get; set; }
            public ClassC ClassC { get; set; }
        }

        class ClassC
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassBId { get; set; }
            [Required]
            public ClassB ClassB { get; set; }
        }

        class ClassD
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassEId { get; set; }
            public ClassE ClassE { get; set; }
        }

        class ClassE
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassDId { get; set; }
            public ClassD ClassD { get; set; }
        }

        class ClassF
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassGId { get; set; }
            public ClassG ClassG { get; set; }
        }

        class ClassG
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ClassFId { get; set; }
            public ClassF ClassF { get; set; }
        }
    }
}
