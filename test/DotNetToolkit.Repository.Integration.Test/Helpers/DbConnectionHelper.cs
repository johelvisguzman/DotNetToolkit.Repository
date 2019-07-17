namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using System.Data.Common;

    public static class DbConnectionHelper
    {
        public static DbConnection CreateConnection()
        {
            var currentFile = PathHelper.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";
            var conn = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0").CreateConnection();

            conn.ConnectionString = connectionString;
            conn.Open();

            return conn;
        }
    }
}
