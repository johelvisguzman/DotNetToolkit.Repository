namespace DotNetToolkit.Repository.Test
{
    using System;
    using Data;
    using Queries;
    using Xunit;

    public class QueryOptionsTests
    {
        [Fact]
        public void Page()
        {
            const int pageIndex = 1;
            const int pageSize = 5;
            const int defaultPageSize = 100;

            IQueryOptions<Customer> options = new QueryOptions<Customer>();

            Assert.Equal(-1, options.PageIndex);
            Assert.Equal(-1, options.PageSize);

            options = options.Page(pageIndex, pageSize);

            Assert.Equal(pageIndex, options.PageIndex);
            Assert.Equal(pageSize, options.PageSize);

            options = options.Page(pageIndex);

            Assert.Equal(pageIndex, options.PageIndex);
            Assert.Equal(pageSize, options.PageSize);

            var ex = Assert.Throws<ArgumentException>(() => options.Page(0, 1));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => options.Page(1, 0));
            Assert.Equal("Cannot be lower than zero.\r\nParameter name: pageSize", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => options.Page(0));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);

            options = new QueryOptions<Customer>().Page(pageIndex);

            Assert.Equal(pageIndex, options.PageIndex);
            Assert.Equal(defaultPageSize, options.PageSize);
        }

        [Fact]
        public void SortBy()
        {
            IQueryOptions<Customer> options = new QueryOptions<Customer>();

            Assert.Null(options.SortingProperty);
            Assert.False(options.IsDescendingSorting);

            options = options.SortBy(x => x.Id);

            Assert.Equal("Id", options.SortingProperty);
            Assert.False(options.IsDescendingSorting);

            options = options.SortBy("Id");

            Assert.Equal("Id", options.SortingProperty);
            Assert.False(options.IsDescendingSorting);
        }

        [Fact]
        public void SortByDescending()
        {
            IQueryOptions<Customer>  options = new QueryOptions<Customer>()
                .SortByDescending(x => x.Id);

            Assert.Equal("Id", options.SortingProperty);
            Assert.True(options.IsDescendingSorting);

            options = options.SortByDescending("Id");

            Assert.Equal("Id", options.SortingProperty);
            Assert.True(options.IsDescendingSorting);
        }
    }
}
