namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;
    using System.Threading.Tasks;

    [BenchmarkCategory("RepositoryTraits")]
    public class Repository_Delete_Benchmarks : BenchmarkBase
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

        [BenchmarkCategory("Delete"), Benchmark]
        public void Delete()
        {
            _repo.Delete(_customer);
        }

        [BenchmarkCategory("DeleteRange"), Benchmark]
        public void DeleteRange()
        {
            _repo.Delete(new[] { _customer });
        }

        [BenchmarkCategory("DeleteWithId"), Benchmark]
        public void DeleteWithId()
        {
            _repo.Delete(_customer.Id);
        }

        [BenchmarkCategory("DeleteWithPredicate"), Benchmark]
        public void DeleteWithPredicate()
        {
            _repo.Delete(x => x.Id == _customer.Id);
        }

        [BenchmarkCategory("DeleteAsync"), Benchmark]
        public async Task Async_Delete()
        {
            await _repo.DeleteAsync(_customer);
        }

        [BenchmarkCategory("DeleteRangeAsync"), Benchmark]
        public async Task Async_DeleteRange()
        {
            await _repo.DeleteAsync(new[] { _customer });
        }

        [BenchmarkCategory("DeleteWithIdAsync"), Benchmark]
        public async Task Async_DeleteWithId()
        {
            await _repo.DeleteAsync(_customer.Id);
        }

        [BenchmarkCategory("DeleteWithPredicateAsync"), Benchmark]
        public async Task Async_DeleteWithPredicate()
        {
            await _repo.DeleteAsync(x => x.Id == _customer.Id);
        }
    }
}
