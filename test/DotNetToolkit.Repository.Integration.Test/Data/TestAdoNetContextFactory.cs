namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AdoNet;
    using System.Data.Common;

    public class TestAdoNetContextFactory
    {
        public static AdoNetRepositoryContextFactory Create()
        {
            return new AdoNetRepositoryContextFactory(CreateConnection());
        }

        public static DbConnection CreateConnection()
        {
            var currentFile = TestPathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            return conn;
        }
    }
}
