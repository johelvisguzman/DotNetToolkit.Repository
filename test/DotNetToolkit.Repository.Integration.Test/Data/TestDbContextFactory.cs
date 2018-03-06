namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Data.Common;
    using System.IO;

    public static class TestDbContextFactory
    {
        public static TestDbContext Create()
        {
            var currentFile = Path.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            var context = new TestDbContext(conn);

            context.Database.CreateIfNotExists();

            return context;
        }
    }
}
