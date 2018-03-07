namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class SortingOptionsTests
    {
        [Fact]
        public void Descending_Sort()
        {
            var entities = new List<Customer>();
            for (var i = 10; i >= 1; i--)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            var sortingOptions = new SortingOptions<Customer, string>(x => x.Name);
            var queryable = sortingOptions.Apply(entities.AsQueryable());

            Assert.Equal("Random Name 1", queryable.First().Name);

            Assert.Equal(10, queryable.Count());
        }

        [Fact]
        public void Ascending_Sort()
        {
            var entities = new List<Customer>();
            for (var i = 10; i >= 1; i--)
            {
                entities.Add(new Customer { Name = "Random Name " + i });
            }

            var sortingOptions = new SortingOptions<Customer, string>(x => x.Name, true);
            var queryable = sortingOptions.Apply(entities.AsQueryable());

            Assert.Equal("Random Name 9", queryable.First().Name);

            Assert.Equal(10, queryable.Count());
        }
    }
}
