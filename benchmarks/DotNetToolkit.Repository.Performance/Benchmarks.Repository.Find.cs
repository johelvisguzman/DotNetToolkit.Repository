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

        [BenchmarkCategory("Find"), Benchmark]
        public void FindWithId()
        {
            _repo.Find(_customer.Id);
        }

        [BenchmarkCategory("Find"), Benchmark]
        public void FindWithPredicate()
        {
            _repo.Find(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("Find"), Benchmark]
        public void FindWithDefaultOptions()
        {
            _repo.Find(_defaultOptions);
        }

        [BenchmarkCategory("Find"), Benchmark]
        public void FindWithPagingOptions()
        {
            _repo.Find(_pagingOptions);
        }

        [BenchmarkCategory("FindAll"), Benchmark]
        public void FindAllWithPredicate()
        {
            _repo.FindAll(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindAll"), Benchmark]
        public void FindAllWithDefaultOptions()
        {
            _repo.FindAll(_defaultOptions);
        }

        [BenchmarkCategory("FindAll"), Benchmark]
        public void FindAllWithPagingOptions()
        {
            _repo.FindAll(_pagingOptions);
        }

        [BenchmarkCategory("ToDictionary"), Benchmark]
        public void ToDictionaryWithDefaultOptions()
        {
            _repo.ToDictionary(_defaultOptions, x => x.Id);
        }

        [BenchmarkCategory("ToDictionary"), Benchmark]
        public void ToDictionaryWithPagingOptions()
        {
            _repo.ToDictionary(_pagingOptions, x => x.Id);
        }

        [BenchmarkCategory("GroupBy"), Benchmark]
        public void GroupByWithDefaultOptions()
        {
            _repo.GroupBy(_defaultOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("GroupBy"), Benchmark]
        public void GroupByWithPagingOptions()
        {
            _repo.GroupBy(_pagingOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("FindAsync"), Benchmark]
        public async Task Async_FindWithId()
        {
            await _repo.FindAsync(_customer.Id);
        }

        [BenchmarkCategory("FindAsync"), Benchmark]
        public async Task Async_FindWithPredicate()
        {
            await _repo.FindAsync(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindAsync"), Benchmark]
        public async Task Async_FindWithDefaultOptions()
        {
            await _repo.FindAsync(_defaultOptions);
        }

        [BenchmarkCategory("FindAsync"), Benchmark]
        public async Task Async_FindWithPagingOptions()
        {
            await _repo.FindAsync(_pagingOptions);
        }

        [BenchmarkCategory("FindAllAsync"), Benchmark]
        public async Task Async_FindAllWithPredicate()
        {
            await _repo.FindAllAsync(x => x.Name.Equals("Random Name"));
        }

        [BenchmarkCategory("FindAllAsync"), Benchmark]
        public async Task Async_FindAllWithDefaultOptions()
        {
            await _repo.FindAllAsync(_defaultOptions);
        }

        [BenchmarkCategory("FindAllAsync"), Benchmark]
        public async Task Async_FindAllWithPagingOptions()
        {
            await _repo.FindAllAsync(_pagingOptions);
        }

        [BenchmarkCategory("GroupByAsync"), Benchmark]
        public async Task Async_GroupByWithDefaultOptions()
        {
            await _repo.GroupByAsync(_defaultOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("GroupByAsync"), Benchmark]
        public async Task Async_GroupByWithPagingOptions()
        {
            await _repo.GroupByAsync(_pagingOptions, y => y.Id, (key, g) => key);
        }

        [BenchmarkCategory("ToDictionaryAsync"), Benchmark]
        public async Task Async_ToDictionaryWithDefaultOptions()
        {
            await _repo.ToDictionaryAsync(_defaultOptions, x => x.Id);
        }

        [BenchmarkCategory("ToDictionaryAsync"), Benchmark]
        public async Task Async_ToDictionaryWithPagingOptions()
        {
            await _repo.ToDictionaryAsync(_pagingOptions, x => x.Id);
        }
    }
}
