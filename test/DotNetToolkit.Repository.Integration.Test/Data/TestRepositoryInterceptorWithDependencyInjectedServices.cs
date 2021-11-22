namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Interceptors;
    using System;
    using Transactions;

    public class TestRepositoryInterceptorWithDependencyInjectedServices : RepositoryInterceptorBase
    {
        public TestRepositoryInterceptorWithDependencyInjectedServices(
                IRepositoryOptions options,
                IRepositoryFactory repoFactory,
                IRepository<Customer> repo,
                IUnitOfWorkFactory uowFactory,
                IUnitOfWork uow)
        {
            if (options == null ||
                repoFactory == null ||
                repo == null ||
                uowFactory == null ||
                uow == null)
                throw new ArgumentNullException();
        }
    }
}
