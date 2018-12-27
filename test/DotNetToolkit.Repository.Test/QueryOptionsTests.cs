namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Helpers;
    using Queries;
    using Queries.Strategies;
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

            Assert.Equal(-1, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(-1, ((IQueryOptions<Customer>)options).PageSize);

            options = options.Page(pageIndex, pageSize);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(pageSize, ((IQueryOptions<Customer>)options).PageSize);

            options = options.Page(pageIndex);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(pageSize, ((IQueryOptions<Customer>)options).PageSize);

            var ex = Assert.Throws<ArgumentException>(() => options.Page(0, 1));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => options.Page(1, 0));
            Assert.Equal("Cannot be lower than zero.\r\nParameter name: pageSize", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => options.Page(0));
            Assert.Equal("Cannot be lower than 1.\r\nParameter name: pageIndex", ex.Message);

            options = new QueryOptions<Customer>().Page(pageIndex);

            Assert.Equal(pageIndex, ((IQueryOptions<Customer>)options).PageIndex);
            Assert.Equal(defaultPageSize, ((IQueryOptions<Customer>)options).PageSize);
        }

        [Fact]
        public void SortBy()
        {
            var options = new QueryOptions<Customer>();

            Assert.Empty(((IQueryOptions<Customer>)options).SortingPropertiesMapping);

            options = options.OrderBy(x => x.Id);

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Value);

            options = options.OrderBy("Id");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortByDescending()
        {
            var options = new QueryOptions<Customer>();

            Assert.Empty(((IQueryOptions<Customer>)options).SortingPropertiesMapping);

            options = options.OrderByDescending(x => x.Id);

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Value);

            options = options.OrderByDescending("Id");

            Assert.Equal(1, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Id", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(0).Value);
        }

        [Fact]
        public void SortThenBy()
        {
            var options = new QueryOptions<Customer>();

            options = options.OrderBy(x => x.Id).OrderBy(x => x.Name);

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Value);

            options = options.OrderBy("Id").OrderBy("Name");

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Ascending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SortThenByDescending()
        {
            var options = new QueryOptions<Customer>();

            options = options.OrderByDescending(x => x.Id).OrderByDescending(x => x.Name);

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Value);

            options = options.OrderByDescending("Id").OrderByDescending("Name");

            Assert.Equal(2, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.Count);
            Assert.Equal("Name", ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Key);
            Assert.Equal(SortOrder.Descending, ((IQueryOptions<Customer>)options).SortingPropertiesMapping.ElementAt(1).Value);
        }

        [Fact]
        public void SatisfyByPredicate()
        {
            Expression<Func<Customer, bool>> firstPredicate = x => x.Name.Equals("Random Name");
            Expression<Func<Customer, bool>> secondPredicate = x => x.Id == 1;

            var options = new QueryOptions<Customer>()
                .SatisfyBy(firstPredicate)
                .SatisfyBy(secondPredicate);

            Assert.Equal(firstPredicate.And(secondPredicate).ToString(), ((IQueryOptions<Customer>)options).SpecificationStrategy.Predicate.ToString());
        }

        [Fact]
        public void SatisfyBySpecification()
        {
            var firstSpec = new SpecificationQueryStrategy<Customer>(x => x.Name.Equals("Random Name"));
            var secondSpec = new SpecificationQueryStrategy<Customer>(x => x.Id == 1);
            var options = new QueryOptions<Customer>()
                .Include(firstSpec)
                .Include(secondSpec);

            Assert.Equal(firstSpec.And(secondSpec).Predicate.ToString(), ((IQueryOptions<Customer>)options).SpecificationStrategy.Predicate.ToString());
        }

        [Fact]
        public void FetchPropertyNames()
        {
            var options = new QueryOptions<Customer>()
                .Fetch("Address")
                .Fetch("Phone")
                .Fetch("Phone.Customer");

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);

            options = new QueryOptions<Customer>()
                .Include(new FetchQueryStrategy<Customer>().Fetch("Address"))
                .Include(new FetchQueryStrategy<Customer>().Fetch("Phone"))
                .Fetch("Phone.Customer");

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
        }

        [Fact]
        public void FetchProperties()
        {
            var options = new QueryOptions<Customer>()
                .Fetch(x => x.Address)
                .Fetch(x => x.Phone)
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);

            options = new QueryOptions<Customer>()
                .Include(new FetchQueryStrategy<Customer>().Fetch(x => x.Address))
                .Include(new FetchQueryStrategy<Customer>().Fetch(x => x.Phone))
                .Fetch(x => x.Phone.Customer);

            Assert.Contains("Address", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
            Assert.Contains("Phone.Customer", ((IQueryOptions<Customer>)options).FetchStrategy.PropertyPaths);
        }

        [Fact]
        public void Options_ToString()
        {
            var options = new QueryOptions<Customer>()
                .Fetch("Address")
                .Fetch("Phone")
                .Fetch("Phone.Customer")
                .SatisfyBy(x => x.Name.Equals("Random Name"))
                .SatisfyBy(x => x.Id > 50)
                .OrderBy("Id")
                .OrderByDescending("Name")
                .Page(1, 10);

            var expected = 
                "QueryOptions<Customer>: [ " +
                "\n\tSpecificationQueryStrategy<Customer>: [ Predicate = x => (x.Name.Equals(\"Random Name\") AndAlso (x.Id > 50)) ]," +
                "\n\tFetchQueryStrategy<Customer>: [ Paths = Address, Phone, Phone.Customer ]," +
                "\n\tSort: [ Id = Ascending, Name = Descending ]," +
                "\n\tPage: [ Index = 1, Size = 10 ]" +
                " ]";

            Assert.Equal(expected, options.ToString());
        }
    }
}
