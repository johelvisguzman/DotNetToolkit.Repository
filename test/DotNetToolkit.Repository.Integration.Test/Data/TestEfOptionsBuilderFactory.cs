namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Options;
    using EntityFramework;
    using System.Data.Common;

    public static class TestEfOptionsBuilderFactory
    {
        public static RepositoryOptionsBuilder Create()
        {
            var currentFile = TestPathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            return new RepositoryOptionsBuilder().UseEntityFramework<TestEfDbContext>(conn);
        }
    }
}
