﻿namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Factories;
    using System;
    using Transactions;

    public class TestRepositoryInterceptorWithDepdencyInjectedServices : RepositoryInterceptorBase
    {
        public TestRepositoryInterceptorWithDepdencyInjectedServices(
                RepositoryOptions options,
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
