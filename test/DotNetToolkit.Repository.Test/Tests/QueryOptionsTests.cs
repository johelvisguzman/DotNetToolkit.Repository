namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Query;
    using Query.Strategies;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Utility;
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

            Assert.Equal(-1, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(-1, ((IQueryOptions<Customer>)options).PageSize);

            options = options.WithPage(pageIndex, pageSize);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(pageSize, ((IQueryOptions<Customer>)options).PageSize);

            options = options.WithPage(pageIndex);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(pageSize, ((IQueryOptions<Customer>)options).PageSize);

            Exception ex;

#if NETFULL
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(0, 1));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(1, 0));
            Assert.Equal("Cannot be lower than zero.\r\nParameter name: pageSize", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(0));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);
#else
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(0, 1));
            Assert.Equal("Cannot be lower than 1. (Parameter 'pageIndex')", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(1, 0));
            Assert.Equal("Cannot be lower than zero. (Parameter 'pageSize')", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => options.WithPage(0));
            Assert.Equal("Cannot be lower than 1. (Parameter 'pageIndex')", ex.Message);
#endif

            options = new QueryOptions<Customer>().WithPage(pageIndex);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(defaultPageSize, ((IQueryOptions<Customer>)options).PageSize);
        }

        [Fact]
        public void SortBy()
        {
            var options = new QueryOptions<Customer>();

            Assert.Empty(((IQueryOptions<Customer>)options).SortingProperties);

            options = options.WithSortBy(x => x.Id);

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);

            options = options.WithSortBy("Id");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);
        }

        [Fact]
        public void SortByDescending()
        {
            var options = new QueryOptions<Customer>();

            Assert.Empty(((IQueryOptions<Customer>)options).SortingProperties);

            options = options.WithSortByDescending(x => x.Id);

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);

            options = options.WithSortByDescending("Id");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);
        }

        [Fact]
        public void SortThenBy()
        {
            var options = new QueryOptions<Customer>();

            options = options.WithSortBy(x => x.Id).WithSortBy(x => x.Name);

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Value);

            options = options.WithSortBy("Id").WithSortBy("Name");

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Value);

            options = new QueryOptions<Customer>().WithSortBy("Name").WithSortByDescending("Name");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);
        }

        [Fact]
        public void SortThenByDescending()
        {
            var options = new QueryOptions<Customer>();

            options = options.WithSortByDescending(x => x.Id).WithSortByDescending(x => x.Name);

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Value);

            options = options.WithSortByDescending("Id").WithSortByDescending("Name");

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(1).Value);

            options = new QueryOptions<Customer>().WithSortByDescending("Name").WithSortBy("Name");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingProperties.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingProperties.ElementAt(0).Value);
        }

        [Fact]
        public void SatisfyByPredicate()
        {
            Expression<Func<Customer, bool>> firstPredicate = x => x.Name.Equals("Random Name");
            Expression<Func<Customer, bool>> secondPredicate = x => x.Id == 1;

            var options = new QueryOptions<Customer>()
                .WithFilter(firstPredicate)
                .WithFilter(secondPredicate);

            Assert.Equal(firstPredicate.And(secondPredicate).ToString(), ((IQueryOptions<Customer>)options).SpecificationStrategy.Predicate.ToString());
        }

        [Fact]
        public void SatisfyBySpecification()
        {
            var firstSpec = new SpecificationQueryStrategy<Customer>(x => x.Name.Equals("Random Name"));
            var secondSpec = new SpecificationQueryStrategy<Customer>(x => x.Id == 1);
            var options = new QueryOptions<Customer>()
                .WithFilter(firstSpec)
                .WithFilter(secondSpec);

            Assert.Equal(firstSpec.And(secondSpec).Predicate.ToString(), ((IQueryOptions<Customer>)options).SpecificationStrategy.Predicate.ToString());
        }

        [Fact]
        public void FetchPropertyNames()
        {
            var options = new QueryOptions<Customer>()
                .WithFetch("Address")
                .WithFetch("Phone")
                .WithFetch("Phone.Customer");

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);

            options = new QueryOptions<Customer>()
                .WithFetch(new FetchQueryStrategy<Customer>().Fetch("Address"))
                .WithFetch(new FetchQueryStrategy<Customer>().Fetch("Phone"))
                .WithFetch("Phone.Customer");

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
        }

        [Fact]
        public void FetchProperties()
        {
            var options = new QueryOptions<Customer>()
                .WithFetch(x => x.Address1)
                .WithFetch(x => x.Phone)
                .WithFetch(x => x.Phone.Customer);

            Assert.Contains("Address1", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);

            options = new QueryOptions<Customer>()
                .WithFetch(new FetchQueryStrategy<Customer>().Fetch(x => x.Address1))
                .WithFetch(new FetchQueryStrategy<Customer>().Fetch(x => x.Phone))
                .WithFetch(x => x.Phone.Customer);

            Assert.Contains("Address1", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
        }

        [Fact]
        public void Options_ToString()
        {
            var options = new QueryOptions<Customer>();

            string expected =
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ null ]," +
                "\n\tFetchQueryStrategy<Customer>: [ null ]," +
                "\n\tSort: [ null ]," +
                "\n\tPage: [ Index = -1, Size = -1 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());

            options = options
                .WithFetch("Address1")
                .WithFetch("Phone")
                .WithFetch("Phone.Customer");

            expected =
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ null ]," +
                "\n\tFetchQueryStrategy<Customer>: [ Paths = Address1, Phone, Phone.Customer ]," +
                "\n\tSort: [ null ]," +
                "\n\tPage: [ Index = -1, Size = -1 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());

            options = options
                .WithFilter(x => x.Name.Equals("Random Name"))
                .WithFilter(x => x.Id > 50);

            expected =
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ Predicate = x => (x.Name.Equals(\"Random Name\") AndAlso (x.Id > 50)) ]," +
                "\n\tFetchQueryStrategy<Customer>: [ Paths = Address1, Phone, Phone.Customer ]," +
                "\n\tSort: [ null ]," +
                "\n\tPage: [ Index = -1, Size = -1 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());

            options = options
                .WithSortBy("Id")
                .WithSortByDescending("Name");

            expected =
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ Predicate = x => (x.Name.Equals(\"Random Name\") AndAlso (x.Id > 50)) ]," +
                "\n\tFetchQueryStrategy<Customer>: [ Paths = Address1, Phone, Phone.Customer ]," +
                "\n\tSort: [ Id = Ascending, Name = Descending ]," +
                "\n\tPage: [ Index = -1, Size = -1 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());

            options = options
                .WithPage(1, 10);

            expected =
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ Predicate = x => (x.Name.Equals(\"Random Name\") AndAlso (x.Id > 50)) ]," +
                "\n\tFetchQueryStrategy<Customer>: [ Paths = Address1, Phone, Phone.Customer ]," +
                "\n\tSort: [ Id = Ascending, Name = Descending ]," +
                "\n\tPage: [ Index = 1, Size = 10 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());
        }
    }
}
