namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;

    [BenchmarkCategory("Delete")]
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

        [Benchmark]
        public void Delete()
        {
            _repo.Delete(_customer);
        }

        [Benchmark]
        public void DeleteRange()
        {
            _repo.Delete(new[] { _customer });
        }

        [Benchmark]
        public void DeleteWithId()
        {
            _repo.Delete(_customer.Id);
        }

        [Benchmark]
        public void DeleteWithPredicate()
        {
            _repo.Delete(x => x.Id == _customer.Id);
        }
    }
}
