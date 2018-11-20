# QueryOptions and QueryResults

The repository and service class each provide a wide range of functionality for fetching data.

In cases where you need to fetch navigation properties (like entity framework [QueryableExtensions.Include](https://msdn.microsoft.com/en-us/library/system.data.entity.queryableextensions.include(v=vs.113).aspx) for example), you would want to use what's called a FetchQueryStrategy object.
For cases where you need to filter the data that is being fetched, then you would use what's called a SpecificationQueryStrategy object.
Each of these strategies can be used as part of the QueryOptions.

Whenever the repository uses a QueryOptions, in most cases, a QueryResults in returned. The QueryResults object holds the result that was returned as well as the total number of rows from the database (this information is obtained in a single select query statement, and can be helpful when doing pagination and need to know the original total count before pagination is applied).

**Fetching Strategy**

```csharp
// **** Define a default fetching strategy ****

var fetchStrategy = new FetchQueryStrategy<Customer>()
  .Fetch(x => x.CustomerAddress)
  .Fetch(x => x.CustomerPhone); // Include multiple navigation properties this way

// **** Define the query options contaning the strategy ****

var queryOptions = new QueryOptions<Customer>()
  .Include(fetchStrategy) // This will include the fetching strategy in the query options
  .Include(new FetchQueryStrategy<Customer>().Fetch(x => x.CustomerOrder)); // Multiple fetching strategies can be combined this way

// **** You can also fetch properties with the query options object itself ****

queryOptions = new QueryOptions<Customer>()
  .Fetch(x => x.CustomerAddress)
  .Fetch(x => x.CustomerPhone); // Include multiple navigation properties this way

// **** Gets the query result ****

var queryResult = repo.FindAll(queryOptions);
```

**Specification Strategy**

```csharp
// **** Define a default specification strategy to with a predicate which is used to filter the data ****

var spec = new SpecificationQueryStrategy<Customer>(x => x.Name = "Random Name");

// **** You can also combine specifications and predicate expressions ****

var combinedSpec = spec
  .And(new SpecificationQueryStrategy<Customer>(x => x.Id > 100)) // combine using a spec
  .Or(x => x.Id == 1); // Combine using a predicate expression

// **** Define the query options contaning the strategy to filter the data ****

var queryOptions = new QueryOptions<Customer>()
  .Include(spec) // This will include the specification strategy in the query options
  .Include(new SpecificationQueryStrategy<Customer>(x => x.Id > 100)); // Multiple specs can be combined this way

// **** You can also set a specification expression predicate on the query options object itself ****

queryOptions = new QueryOptions<Customer>()
  .SatisfyBy(x => x.Name = "Random Name")
  .SatisfyBy(x => x.Id > 100); // Multiple specification expression predicates can be combine this way

// **** Gets the query result ****

var queryResult = repo.FindAll(queryOptions);
```

**Pagination**

```csharp
// **** You can page the data using the query options ****

var queryOptions = new QueryOptions<Customer>()
  .Page(pageInde: 1) // Using a paging index (and a default page size of 100 items)
  .Page(pageInde: 1, pageSize: 10);

// **** Gets the query result ****

// **** The total number is going to hold the actual number of rows before pagination was applied ****
// **** This is being done in a single sql query statement ****

var queryResult = repo.FindAll(queryOptions);
```

**Sorting**

```csharp
// **** You can sort the data using the query options ****

var queryOptions = new QueryOptions<Customer>()
  .SortBy(x => x.Id)
  .SortByDescending(x => x.Name); // Multiple sorting can be applied this way

// **** Gets the query result ****

var queryResult = repo.FindAll(queryOptions);
```
