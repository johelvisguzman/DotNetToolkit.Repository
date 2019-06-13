namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using Queries;

    [BenchmarkCategory("Find")]
    public class Repository_Find_Benchmarks : BenchmarkBase
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
        public void FindWithId()
        {
            _repo.Find(_customer.Id);
        }

        [Benchmark]
        public void FindWithPredicate()
        {
            _repo.Find(x => x.Name.Equals("Random Name"));
        }

        [Benchmark]
        public void FindWithPredicateOptions()
        {
            _repo.Find(_defaultOptions);
        }

        [Benchmark]
        public void FindWithPagingOptions()
        {
            _repo.Find(_pagingOptions);
        }

        [Benchmark]
        public void FindAllWithPredicate()
        {
            _repo.FindAll(x => x.Name.Equals("Random Name"));
        }

        [Benchmark]
        public void FindAllWithPredicateOptions()
        {
            _repo.FindAll(_defaultOptions);
        }

        [Benchmark]
        public void FindAllWithPagingOptions()
        {
            _repo.FindAll(_pagingOptions);
        }
    }
}
