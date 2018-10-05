namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;

    public static class TestEfCoreDbContextFactory
    {
        public static EfCoreRepositoryContextFactory<TestEfCoreDbContext> Create()
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<TestEfCoreDbContext>();

            contextOptionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new EfCoreRepositoryContextFactory<TestEfCoreDbContext>(contextOptionsBuilder.Options);
        }
    }
}