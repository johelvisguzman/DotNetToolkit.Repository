namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;

    public static class TestEfCoreDbContextFactory
    {
        public static EfCoreRepositoryContextFactory<TestEfCoreDbContext> Create()
        {
            return new EfCoreRepositoryContextFactory<TestEfCoreDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        }
    }
}