namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using System.Threading.Tasks;

    [BenchmarkCategory("RepositoryTraits")]
    public class Repository_Update_Benchmarks : BenchmarkBase
    {
        private IRepository<Customer> _repo;
        private Customer _customer;

        [IterationSetup]
        public void Setup()
        {
            BaseSetup();

            _customer = new Customer { Name = "Random Name" };

            _repo = new Repository<Customer>(BuildOptions(Provider));
            _repo.Add(_customer);
        }

        [IterationCleanup]
        public void Clean()
        {
            _repo.Delete(x => x.Id != 0);
        }

        [BenchmarkCategory("Update"), Benchmark]
        public void Update()
        {
            _repo.Update(_customer);
        }

        [BenchmarkCategory("Update"), Benchmark]
        public void UpdateRange()
        {
            _repo.Update(new[] { _customer });
        }

        [BenchmarkCategory("UpdateAsync"), Benchmark]
        public async Task Async_Update()
        {
            await _repo.UpdateAsync(_customer);
        }

        [BenchmarkCategory("UpdateAsync"), Benchmark]
        public async Task Async_UpdateRange()
        {
            await _repo.UpdateAsync(new[] { _customer });
        }
    }
}
