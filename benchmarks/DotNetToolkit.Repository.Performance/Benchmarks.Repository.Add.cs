namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Attributes;
    using Data;

    [BenchmarkCategory("Add")]
    public class Repository_Add_Benchmarks : BenchmarkBase
    {
        private IRepository<Customer> _repo;
        private Customer _customer;

        [IterationSetup]
        public void Setup()
        {
            BaseSetup();

            _customer = new Customer { Id = new System.Random().Next(), Name = "Random Name" };

            _repo = new Repository<Customer>(BuildOptions(Provider));
        }

        [IterationCleanup]
        public void Clean()
        {
            _repo.Delete(x => x.Id != 0);
        }

        [Benchmark]
        public void Add()
        {
            _repo.Add(_customer);
        }

        [Benchmark]
        public void AddRange()
        {
            _repo.Add(new[] { _customer });
        }
    }
}
