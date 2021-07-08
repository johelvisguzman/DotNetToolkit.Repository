namespace DotNetToolkit.Repository.Performance.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using Query;

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

            _customer = new Customer { Id = new System.Random().Next(), Name = "Random Name" };

            _repo = new Repository<Customer>(BuildOptions(Provider));
            _repo.Add(_customer);
        }

        [Benchmark]
        public void GroupByWithPredicateOptions()
        {
            _repo.GroupBy(_defaultOptions, y => y.Id, z => z.Key);
        }

        [Benchmark]
        public void GroupByWithPagingOptions()
        {
            _repo.GroupBy(_pagingOptions, y => y.Id, z => z.Key);
        }
    }
}
