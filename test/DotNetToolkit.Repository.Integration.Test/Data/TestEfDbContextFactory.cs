namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using EntityFramework;
    using EntityFrameworkCore;
    using System;
    using System.Data.Common;

    public static class TestEfDbContextFactory
    {
        public static EfRepositoryContext Create()
        {
            var currentFile = TestPathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            return new EfRepositoryContext(new TestEfDbContext(conn));
        }
    }

    public static class TestEfCoreDbContextFactory
    {
        public static EfCoreRepositoryContext Create()
        {
            return new EfCoreRepositoryContext(new TestEfCoreDbContext(Guid.NewGuid().ToString()));
        }
    }
}
