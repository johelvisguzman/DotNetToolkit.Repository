namespace DotNetToolkit.Repository.Integration.Test.Repository
{
    using System;
    using Data;
    using Query;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class RepositoryTests : TestBase
    {
        [Fact]
        public void GroupBy()
        {
            ForAllRepositoryFactories(TestGroupBy);
        }

        [Fact]
        public void GroupByThrowsExceptionWithSortingOptions()
        {
            ForAllRepositoryFactories(TestGroupByThrowsExceptionWithSortingOptions);
        }

        [Fact]
        public void GroupByAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByAsync);
        }

        [Fact]
        public void GroupByThrowsExceptionWithSortingOptionsAsync()
        {
            ForAllRepositoryFactoriesAsync(TestGroupByThrowsExceptionWithSortingOptionsAsync);
        }

        private static void TestGroupBy(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer { Id = 1, Name = name }
            };

            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, z => z.Key).Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, z => z.Key).Result.Contains(x.Key)));

            repo.Add(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(y => y.Id, z => z.Key).Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupBy(options, y => y.Id, z => z.Key).Result.Contains(x.Key)));
        }

        private static void TestGroupByThrowsExceptionWithSortingOptions(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id);

            var ex = Assert.Throws<InvalidOperationException>(() => repo.GroupBy(options, y => y.Name, z => z.Key));

            Assert.Equal("This context provider does not support groupby operation with sorting.", ex.Message);
        }

        private static async Task TestGroupByAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer { Id = 1, Name = name }
            };

            var expectedGroup = entities.GroupBy(y => y.Id);
            var expectedGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, z => z.Key).Result.Contains(x.Key)));
            Assert.False(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, z => z.Key).Result.Result.Contains(x.Key)));

            await repo.AddAsync(entities);

            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(y => y.Id, z => z.Key).Result.Contains(x.Key)));
            Assert.True(expectedGroupByElementSelector.All(x => repo.GroupByAsync(options, y => y.Id, z => z.Key).Result.Result.Contains(x.Key)));
        }

        private static async Task TestGroupByThrowsExceptionWithSortingOptionsAsync(IRepositoryFactory repoFactory)
        {
            var repo = repoFactory.Create<Customer>();

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GroupByAsync(options, y => y.Name, z => z.Key));

            Assert.Equal("This context provider does not support groupby operation with sorting.", ex.Message);
        }
    }
}
