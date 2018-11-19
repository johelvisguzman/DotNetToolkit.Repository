# QueryOptions and QueryResults

The each repository and service class provide a wide range of functionality for fetching data. 
In cases where you need to fetch navigation properties (like entity framework .Include for example), you would want to use what's called a FetchQueryStrategy object.
For cases where you need to filter the data that is being fetched, then you would use what's called a SpecificationQueryStrategy object.
Each of these strategies can be used as part of the QueryOptions. Whenever the repository uses a QueryOptions, in most cases, a QueryResults in returned.
The QueryResults object holds the result that was returned as well as the total number of rows from the database (this information is obtained in a single select query statement, and can be helpful when doing pagination and need to know the original total count before pagination is applied).

**Fetching Strategy**

```csharp
var repo = new Repository<Customer>(repoOptions);

// Define a default fetching strategy
var fetchStrategy = new FetchQueryStrategy()
  .Fetch(x => x.CustomerAddress)

// Define the query options contaning the strategy
var queryOptions = new QueryOptions<Customer>()
  .Include(fetchStrategy); // This will include the fetching strategy in the query options

// You can also fetch properties with the query options object itself
queryOptions = new QueryOptions<Customer>()
  .Fetch(x => x.CustomerAddress)

var queryResult = repo.FindAll(queryOptions);
```

**Specification Strategy**

```csharp
var repo = new Repository<Customer>(repoOptions);

// Define a default specification strategy to with a predicate which is used to filter the data
var spec = new SpecificationQueryStrategy<Customer>(x => x.Name = "Random Name");

// You can also combine specifications and predicate expressions
var combinedSpec = spec
  .And(new SpecificationQueryStrategy<Customer>(x => x.Id > 100)) // combine using a spec
  .Or(x => x.Id == 1); // combine using a predicate expression

// Define the query options contaning the strategy to filter the data
var queryOptions = new QueryOptions<Customer>()
  .Include(spec); // This will include the specification strategy in the query options

// You can also set a specification expression predicate on the query options object itself
queryOptions = new QueryOptions<Customer>()
  .SatisfyBy(x => x.Name = "Random Name");

var queryResult = repo.FindAll(queryOptions);
```

**Pagination**

```csharp
var repo = new Repository<Customer>(repoOptions);

// You can page the data using the query options
var queryOptions = new QueryOptions<Customer>()
  .Page(pageInde: 1) // using a paging index (and a default page size of 100 items)
  .Page(pageInde: 1, pageSize: 10);
  
var queryResult = repo.FindAll(queryOptions);
```

**Sorting**

```csharp
var repo = new Repository<Customer>(repoOptions);

// You can sort the data using the query options
var queryOptions = new QueryOptions<Customer>()
  .SortBy(x => x.Id)
  .SortByDescending(x => x.Name); // multiple sorting can be applied this way
  
var queryResult = repo.FindAll(queryOptions);
```
