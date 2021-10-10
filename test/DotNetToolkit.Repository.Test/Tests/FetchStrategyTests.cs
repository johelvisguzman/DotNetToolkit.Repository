namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Extensions.Internal;
    using Query.Strategies;
    using Xunit;

    public class FetchStrategyTests
    {
        [Fact]
        public void FetchPropertiesByExpression()
        {
            var strategy = new FetchQueryStrategy<Customer>()
                .Fetch(x => x.Address1)
                .Fetch(x => x.Phone)
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address1", strategy.PropertyPaths);
            Assert.Contains("Phone", strategy.PropertyPaths);
            Assert.Contains("Phone.Customer", strategy.PropertyPaths);
        }

        [Fact]
        public void FetchPropertiesByName()
        {
            var strategy = new FetchQueryStrategy<Customer>()
                .Fetch("Address1")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Contains("Address1", strategy.PropertyPaths);
            Assert.Contains("Phone", strategy.PropertyPaths);
            Assert.Contains("Phone.Customer", strategy.PropertyPaths);
        }

        [Fact]
        public void FetchToString()
        {
            var strategy = new FetchQueryStrategy<Customer>()
                .Fetch("Address1")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Equal("FetchQueryStrategy<Customer>: [ Paths = Address1, Phone, Phone.Customer ]", strategy.ToString());
        }

        [Fact]
        public void NormalizePropertyPaths()
        {
            var paths = new string[]
            {
                "A.B.C",
                "A.B.C.D",
                "A.B",
                "A",
                "A.D.E",
                "A.D",
                "A.G",
                "A.G.H",
                "T",
                "A.G.H.I",
            };

            var normalized = paths.NormalizePropertyPaths();

            Assert.Equal(4, normalized.Length);
            Assert.Equal("A.B.C.D", normalized[0]);
            Assert.Equal("T", normalized[1]);
            Assert.Equal("A.D.E", normalized[2]);
            Assert.Equal("A.G.H.I", normalized[3]);

        }

        [Fact]
        public void UnableToNormalizePropertyPathsOutOfSequence()
        {
            var paths = new string[]
            {
                "A",
                "A.B.C.D",
            };

            var normalized = paths.NormalizePropertyPaths();

            Assert.Single(normalized);
            Assert.Equal("A", normalized[0]);

        }
    }
}
