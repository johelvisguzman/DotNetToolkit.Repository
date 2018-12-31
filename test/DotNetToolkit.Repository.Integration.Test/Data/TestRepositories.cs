namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Options;

    public interface ITestCustomerRepository : IRepository<Customer> { }

    public class TestCustomerRepository : RepositoryBase<Customer, int>, ITestCustomerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public TestCustomerRepository(IRepositoryOptions options) : base(options) { }
    }
}
