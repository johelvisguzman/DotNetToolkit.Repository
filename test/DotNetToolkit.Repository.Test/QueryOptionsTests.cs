namespace DotNetToolkit.Repository.Test
{
    using Data;
    using FetchStrategies;
    using Queries;
    using Specifications;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
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

            Assert.Empty(options.SortingPropertiesMapping);

            options = options.SortBy(x => x.Id);

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.False(options.SortingPropertiesMapping.ElementAt(0).Value);

            options = options.SortBy("Id");

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.False(options.SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortByDescending()
        {
            IQueryOptions<Customer> options = new QueryOptions<Customer>();

            Assert.Empty(options.SortingPropertiesMapping);

            options = options.SortByDescending(x => x.Id);

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.True(options.SortingPropertiesMapping.ElementAt(0).Value);

            options = options.SortByDescending("Id");

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.True(options.SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortThenBy()
        {
            IQueryOptions<Customer> options = new QueryOptions<Customer>();

            var ex = Assert.Throws<InvalidOperationException>(() => options.ThenSortBy("Name"));
            Assert.Equal("Cannot perform sorting action. A primary sorting will need to be applied first.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => options.ThenSortBy(x => x.Name));
            Assert.Equal("Cannot perform sorting action. A primary sorting will need to be applied first.", ex.Message);

            options = options.SortBy(x => x.Id).ThenSortBy(x => x.Name);

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.False(options.SortingPropertiesMapping.ElementAt(1).Value);

            options = options.SortBy("Id").ThenSortBy("Name");

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.False(options.SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SortThenByDescending()
        {
            IQueryOptions<Customer> options = new QueryOptions<Customer>();

            var ex = Assert.Throws<InvalidOperationException>(() => options.ThenSortByDescending("Name"));
            Assert.Equal("Cannot perform sorting action. A primary sorting will need to be applied first.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => options.ThenSortByDescending(x => x.Name));
            Assert.Equal("Cannot perform sorting action. A primary sorting will need to be applied first.", ex.Message);

            options = options.SortByDescending(x => x.Id).ThenSortByDescending(x => x.Name);

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.True(options.SortingPropertiesMapping.ElementAt(1).Value);

            options = options.SortByDescending("Id").ThenSortByDescending("Name");

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.True(options.SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SatisfyBy()
        {
            Expression<Func<Customer, bool>> predicate = x => true;

            var spec = new Specification<Customer>(predicate);

            Assert.Null(new QueryOptions<Customer>().Specification);
            Assert.Equal(spec, new QueryOptions<Customer>().SatisfyBy(spec).Specification);
            Assert.Equal(predicate, new QueryOptions<Customer>().SatisfyBy(predicate).Specification.Predicate);
        }

        [Fact]
        public void FetchPropertyNames()
        {
            var fetchStrategy = new FetchStrategy<Customer>();

            var options = new QueryOptions<Customer>()
                .Fetch(fetchStrategy)
                .Fetch("Address")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Equal(fetchStrategy, options.FetchStrategy);

            Assert.Contains("Address", fetchStrategy.IncludePaths);
            Assert.Contains("Phone", fetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", fetchStrategy.IncludePaths);
        }

        [Fact]
        public void FetchProperties()
        {
            var fetchStrategy = new FetchStrategy<Customer>();

            var options = new QueryOptions<Customer>()
                .Fetch(fetchStrategy)
                .Fetch(x => x.Address)
                .Fetch(x => x.Phone)
                .Fetch(x => x.Phone.Customer);

            Assert.Equal(fetchStrategy, options.FetchStrategy);

            Assert.Contains("Address", fetchStrategy.IncludePaths);
            Assert.Contains("Phone", fetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", fetchStrategy.IncludePaths);
        }
    }
}
