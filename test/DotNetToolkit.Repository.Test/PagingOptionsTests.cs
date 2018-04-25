namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Queries;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class PagingOptionsTests
    {
        [Fact]
        public void Page_With_Sort_Ascending()
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id);
            var queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 0", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 1", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 2", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 3", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 4", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 5", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 6", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 7", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 8", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 9", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 10", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 11", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 12", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 13", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 14", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 15", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 16", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 17", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 18", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 19", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Single(queryable);
            Assert.Equal("Random Name 20", queryable.ElementAt(0).Name);
        }

        [Fact]
        public void Page_With_Sort_Descending()
        {
            var entities = new List<Customer>();

            for (var i = 0; i < 21; i++)
            {
                entities.Add(new Customer { Id = i, Name = "Random Name " + i });
            }

            var queryOptions = new PagingOptions<Customer, int>(1, 5, x => x.Id, true);
            var queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 20", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 19", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 18", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 17", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 16", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 2;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 15", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 14", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 13", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 12", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 11", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 3;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 10", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 9", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 8", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 7", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 6", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 4;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Equal(5, queryable.Count());
            Assert.Equal("Random Name 5", queryable.ElementAt(0).Name);
            Assert.Equal("Random Name 4", queryable.ElementAt(1).Name);
            Assert.Equal("Random Name 3", queryable.ElementAt(2).Name);
            Assert.Equal("Random Name 2", queryable.ElementAt(3).Name);
            Assert.Equal("Random Name 1", queryable.ElementAt(4).Name);

            queryOptions.PageIndex = 5;

            queryable = queryOptions.Apply(entities.AsQueryable());

            Assert.Single(queryable);
            Assert.Equal("Random Name 0", queryable.ElementAt(0).Name);
        }
    }
}
