namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Queries.Strategies;
    using Xunit;

    public class FetchStrategyTests
    {
        [Fact]
        public void Fetch()
        {
            var strategy = new FetchQueryStrategy<Customer>()
                .Fetch(x => x.Address)
                .Fetch(x => x.Phone)
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address", strategy.PropertyPaths);
            Assert.Contains("Phone", strategy.PropertyPaths);
            Assert.Contains("Phone.Customer", strategy.PropertyPaths);
        }

        [Fact]
        public void Fetch_Property_Names()
        {
            var strategy = new FetchQueryStrategy<Customer>()
                .Fetch("Address")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Contains("Address", strategy.PropertyPaths);
            Assert.Contains("Phone", strategy.PropertyPaths);
            Assert.Contains("Phone.Customer", strategy.PropertyPaths);
        }
    }
}
