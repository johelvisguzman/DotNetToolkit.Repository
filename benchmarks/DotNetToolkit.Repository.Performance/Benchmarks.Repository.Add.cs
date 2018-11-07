namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using System.Threading.Tasks;

    [BenchmarkCategory("RepositoryTraits")]
    public class Repository_Add_Benchmarks : BenchmarkBase
    {
        private IRepository<Customer> _repo;
        
        [IterationSetup]
        public void Setup()
        {
            BaseSetup();

            _repo = new Repository<Customer>(BuildOptions(Provider));
        }

        [IterationCleanup]
        public void Clean()
        {
            _repo.Delete(x => x.Id != 0);
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void Add()
        {
            _repo.Add(new Customer { Name = "Random Name" });
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void AddRange()
        {
            _repo.Add(new[] { new Customer { Name = "Random Name" } });
        }

        [BenchmarkCategory("AddAsync"), Benchmark]
        public async Task Async_Add()
        {
            await _repo.AddAsync(new Customer { Name = "Random Name" });
        }

        [BenchmarkCategory("AddAsync"), Benchmark]
        public async Task Async_AddRange()
        {
            await _repo.AddAsync(new[] { new Customer { Name = "Random Name" } });
        }
    }
}
