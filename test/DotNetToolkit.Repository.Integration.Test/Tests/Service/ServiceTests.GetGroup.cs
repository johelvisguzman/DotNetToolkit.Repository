namespace DotNetToolkit.Repository.Integration.Test.Service
{
    using System;
    using Data;
    using Query;
    using Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class ServiceTests : TestBase
    {
        [Fact]
        public void GetGroupBy()
        {
            ForAllServiceFactories(TestGetGroupBy);
        }

        [Fact]
        public void GetGroupByThrowsExceptionWithSortingOptions()
        {
            ForAllServiceFactories(TestGetGroupByThrowsExceptionWithSortingOptions);
        }

        [Fact]
        public void GetGroupByAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByAsync);
        }

        [Fact]
        public void GetGroupByThrowsExceptionWithSortingOptionsAsync()
        {
            ForAllServiceFactoriesAsync(TestGetGroupByThrowsExceptionWithSortingOptionsAsync);
        }

        private static void TestGetGroupBy(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGetGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(y => y.Id, z => z.Key).Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(options, y => y.Id, z => z.Key).Result.Contains(x.Key)));

            service.Create(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(y => y.Id, z => z.Key).Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupBy(options, y => y.Id, z => z.Key).Result.Contains(x.Key)));
        }

        private static void TestGetGroupByThrowsExceptionWithSortingOptions(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id);

            var ex = Assert.Throws<InvalidOperationException>(() => service.GetGroupBy(options, y => y.Name, z => z.Key));

            Assert.Equal("This context provider does not support groupby operation with sorting.", ex.Message);
        }

        private static async Task TestGetGroupByAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            const string name = "Random Name";

            var options = new QueryOptions<Customer>();
            var entities = new List<Customer>()
            {
                new Customer {Name = name}
            };

            var expectedGetGroupByElementSelector = entities.GroupBy(y => y.Id, y => y.Name);

            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(y => y.Id, z => z.Key).Result.Contains(x.Key)));
            Assert.False(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(options, y => y.Id, z => z.Key).Result.Result.Contains(x.Key)));

            await service.CreateAsync(entities);

            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(y => y.Id, z => z.Key).Result.Contains(x.Key)));
            Assert.True(expectedGetGroupByElementSelector.All(x =>
                service.GetGroupByAsync(options, y => y.Id, z => z.Key).Result.Result.Contains(x.Key)));
        }

        private static async Task TestGetGroupByThrowsExceptionWithSortingOptionsAsync(IServiceFactory serviceFactory)
        {
            var service = serviceFactory.Create<Customer>();

            var options = new QueryOptions<Customer>().OrderBy(x => x.Id);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGroupByAsync(options, y => y.Name, z => z.Key));

            Assert.Equal("This context provider does not support groupby operation with sorting.", ex.Message);
        }
    }
}