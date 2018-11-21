namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AdoNet;
    using Configuration.Options;
    using System.Data.Common;
    using DbProviderFactories = System.Data.Common.DbProviderFactories;

    public class TestAdoNetOptionsBuilderFactory
    {
        public static RepositoryOptionsBuilder Create()
        {
            return new RepositoryOptionsBuilder().UseAdoNet(CreateConnection());
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
