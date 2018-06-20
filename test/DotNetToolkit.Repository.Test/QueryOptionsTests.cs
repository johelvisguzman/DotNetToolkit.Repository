namespace DotNetToolkit.Repository.Test
{
    using Data;
    using FetchStrategies;
    using Helpers;
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

            var options = new QueryOptions<Customer>();

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
            var options = new QueryOptions<Customer>();

            Assert.Empty(options.SortingPropertiesMapping);

            options = options.SortBy(x => x.Id);

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, options.SortingPropertiesMapping.ElementAt(0).Value);

            options = options.SortBy("Id");

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, options.SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortByDescending()
        {
            var options = new QueryOptions<Customer>();

            Assert.Empty(options.SortingPropertiesMapping);

            options = options.SortByDescending(x => x.Id);

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, options.SortingPropertiesMapping.ElementAt(0).Value);

            options = options.SortByDescending("Id");

            Assert.Equal(1, options.SortingPropertiesMapping.Count);
            Assert.Equal("Id", options.SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, options.SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortThenBy()
        {
            var options = new QueryOptions<Customer>();

            options = options.SortBy(x => x.Id).SortBy(x => x.Name);

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, options.SortingPropertiesMapping.ElementAt(1).Value);

            options = options.SortBy("Id").SortBy("Name");

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, options.SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SortThenByDescending()
        {
            var options = new QueryOptions<Customer>();

            options = options.SortByDescending(x => x.Id).SortByDescending(x => x.Name);

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, options.SortingPropertiesMapping.ElementAt(1).Value);

            options = options.SortByDescending("Id").SortByDescending("Name");

            Assert.Equal(2, options.SortingPropertiesMapping.Count);
            Assert.Equal("Name", options.SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, options.SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SatisfyByPredicate()
        {
            Expression<Func<Customer, bool>> firstPredicate = x => x.Name.Equals("Random Name");
            Expression<Func<Customer, bool>> secondPredicate = x => x.Id == 1;

            var options = new QueryOptions<Customer>()
                .SatisfyBy(firstPredicate)
                .SatisfyBy(secondPredicate);

            Assert.Equal(firstPredicate.And(secondPredicate).ToString(), options.Specification.Predicate.ToString());
        }

        [Fact]
        public void SatisfyBySpecification()
        {
            var firstSpec = new Specification<Customer>(x => x.Name.Equals("Random Name"));
            var secondSpec = new Specification<Customer>(x => x.Id == 1);
            var options = new QueryOptions<Customer>()
                .SatisfyBy(firstSpec)
                .SatisfyBy(secondSpec);

            Assert.Equal(firstSpec.And(secondSpec).Predicate.ToString(), options.Specification.Predicate.ToString());
        }

        [Fact]
        public void FetchPropertyNames()
        {
            var options = new QueryOptions<Customer>()
                .Fetch("Address")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Contains("Address", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", options.FetchStrategy.IncludePaths);

            options = new QueryOptions<Customer>()
                .Fetch(new FetchStrategy<Customer>().Include("Address"))
                .Fetch(new FetchStrategy<Customer>().Include("Phone"))
                .Fetch("Phone.Customer");

            Assert.Contains("Address", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", options.FetchStrategy.IncludePaths);
        }

        [Fact]
        public void FetchProperties()
        {
            var options = new QueryOptions<Customer>()
                .Fetch(x => x.Address)
                .Fetch(x => x.Phone)
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", options.FetchStrategy.IncludePaths);

            options = new QueryOptions<Customer>()
                .Fetch(new FetchStrategy<Customer>().Include(x => x.Address))
                .Fetch(new FetchStrategy<Customer>().Include(x => x.Phone))
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone", options.FetchStrategy.IncludePaths);
            Assert.Contains("Phone.Customer", options.FetchStrategy.IncludePaths);
        }
    }
}
