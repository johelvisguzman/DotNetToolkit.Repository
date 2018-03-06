namespace DotNetToolkit.Repository.Test
{
    using Data;
    using FetchStrategies;
    using Xunit;

    public class FetchStrategyTests
    {
        [Fact]
        public void Include()
        {
            var strategy = new FetchStrategy<Customer>()
                .Include(x => x.Address)
                .Include(x => x.Phone)
                .Include(x => x.Phone.Customer);

            Assert.Contains("Address", strategy.IncludePaths);
            Assert.Contains("Phone", strategy.IncludePaths);
            Assert.Contains("Phone.Customer", strategy.IncludePaths);
        }

        [Fact]
        public void Include_Property_Names()
        {
            var strategy = new FetchStrategy<Customer>()
                .Include("Address")
                .Include("Phone")
                .Include("Phone.Customer");

            Assert.Contains("Address", strategy.IncludePaths);
            Assert.Contains("Phone", strategy.IncludePaths);
            Assert.Contains("Phone.Customer", strategy.IncludePaths);
        }
    }
}
