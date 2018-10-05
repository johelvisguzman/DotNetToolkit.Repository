namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Collections.Generic;
    using Configuration;
    using Configuration.Interceptors;
    using Factories;

    public interface ITestCustomerRepository : IRepository<Customer> { }

    public class TestCustomerRepository : RepositoryBase<Customer, int>, ITestCustomerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="factory">The context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public TestCustomerRepository(IRepositoryContextFactory factory, IEnumerable<IRepositoryInterceptor> interceptors) : base(factory, interceptors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public TestCustomerRepository(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors)
        {
        }
    }
}
