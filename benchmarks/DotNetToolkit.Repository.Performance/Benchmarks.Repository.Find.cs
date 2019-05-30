namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using Queries;
    using System.Threading.Tasks;

    [BenchmarkCategory("RepositoryTraits")]
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

        [BenchmarkCategory("FindWithId"), Benchmark]
        public void FindWithId()
        {
            _repo.Find(_customer.Id);
        }

        [BenchmarkCategory("FindWithPredicate"), Benchmark]
        public void FindWithPredicate()
        {
            _repo.Find(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindWithPredicateOptions"), Benchmark]
        public void FindWithPredicateOptions()
        {
            _repo.Find(_defaultOptions);
        }

        [BenchmarkCategory("FindWithPagingOptions"), Benchmark]
        public void FindWithPagingOptions()
        {
            _repo.Find(_pagingOptions);
        }

        [BenchmarkCategory("FindAllWithPredicate"), Benchmark]
        public void FindAllWithPredicate()
        {
            _repo.FindAll(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindAllWithPredicateOptions"), Benchmark]
        public void FindAllWithPredicateOptions()
        {
            _repo.FindAll(_defaultOptions);
        }

        [BenchmarkCategory("FindAllWithPagingOptions"), Benchmark]
        public void FindAllWithPagingOptions()
        {
            _repo.FindAll(_pagingOptions);
        }

        [BenchmarkCategory("ToDictionaryWithPredicateOptions"), Benchmark]
        public void ToDictionaryWithPredicateOptions()
        {
            _repo.ToDictionary(_defaultOptions, x => x.Id);
        }

        [BenchmarkCategory("ToDictionaryWithPagingOptions"), Benchmark]
        public void ToDictionaryWithPagingOptions()
        {
            _repo.ToDictionary(_pagingOptions, x => x.Id);
        }

        [BenchmarkCategory("GroupByWithPredicateOptions"), Benchmark]
        public void GroupByWithPredicateOptions()
        {
            _repo.GroupBy(_defaultOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("GroupByWithPagingOptions"), Benchmark]
        public void GroupByWithPagingOptions()
        {
            _repo.GroupBy(_pagingOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("FindWithIdAsync"), Benchmark]
        public async Task Async_FindWithId()
        {
            await _repo.FindAsync(_customer.Id);
        }

        [BenchmarkCategory("FindWithPredicateAsync"), Benchmark]
        public async Task Async_FindWithPredicate()
        {
            await _repo.FindAsync(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindWithPreicateOptionsAsync"), Benchmark]
        public async Task Async_FindWithPreicateOptions()
        {
            await _repo.FindAsync(_defaultOptions);
        }

        [BenchmarkCategory("FindWithPagingOptionsAsync"), Benchmark]
        public async Task Async_FindWithPagingOptions()
        {
            await _repo.FindAsync(_pagingOptions);
        }

        [BenchmarkCategory("FindAllWithPredicateAsync"), Benchmark]
        public async Task Async_FindAllWithPredicate()
        {
            await _repo.FindAllAsync(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindAllWithPredicateOptionsAsync"), Benchmark]
        public async Task Async_FindAllWithPredicateOptions()
        {
            await _repo.FindAllAsync(_defaultOptions);
        }

        [BenchmarkCategory("FindAllWithPagingOptionsAsync"), Benchmark]
        public async Task Async_FindAllWithPagingOptions()
        {
            await _repo.FindAllAsync(_pagingOptions);
        }

        [BenchmarkCategory("GroupByWithPredicateOptionsAsync"), Benchmark]
        public async Task Async_GroupByWithPredicateOptions()
        {
            await _repo.GroupByAsync(_defaultOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("GroupByWithPagingOptionsAsync"), Benchmark]
        public async Task Async_GroupByWithPagingOptions()
        {
            await _repo.GroupByAsync(_pagingOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("ToDictionaryWithPredicateOptionsAsync"), Benchmark]
        public async Task Async_ToDictionaryWithPredicateOptions()
        {
            await _repo.ToDictionaryAsync(_defaultOptions, x => x.Id);
        }

        [BenchmarkCategory("ToDictionaryWithPagingOptionsAsync"), Benchmark]
        public async Task Async_ToDictionaryWithPagingOptions()
        {
            await _repo.ToDictionaryAsync(_pagingOptions, x => x.Id);
        }
    }
}
