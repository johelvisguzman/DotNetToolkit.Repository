namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using EntityFramework;
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Data.Common;

    public static class TestEfDbContextFactory
    {
        public static EfRepositoryContextFactory<TestEfDbContext> Create()
        {
            var currentFile = TestPathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            return new EfRepositoryContextFactory<TestEfDbContext>(conn);
        }
    }

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
