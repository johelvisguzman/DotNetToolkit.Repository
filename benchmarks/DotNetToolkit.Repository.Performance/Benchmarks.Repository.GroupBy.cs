namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using Queries;

    [BenchmarkCategory("GroupBy")]
    public class Repository_GroupBy_Benchmarks : BenchmarkBase
    {
        private IRepository<Customer> _repo;
        private Customer _customer;
        private QueryOptions<Customer> _defaultOptions;
        private QueryOptions<Customer> _pagingOptions;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            _defaultOptions = new QueryOptions<Customer>()
                .SatisfyBy(x => x.Name.Equals("Random Name"));

            _pagingOptions = _defaultOptions.Page(1, 10);

            _customer = new Customer { Name = "Random Name" };

            _repo = new Repository<Customer>(BuildOptions(Provider));
            _repo.Add(_customer);
        }

        [Benchmark]
        public void GroupByWithPredicateOptions()
        {
            _repo.GroupBy(_defaultOptions, y => y.Id, (key, g) => key);
        }

        [Benchmark]
        public void GroupByWithPagingOptions()
        {
            _repo.GroupBy(_pagingOptions, y => y.Id, (key, g) => key);
        }
    }
}
