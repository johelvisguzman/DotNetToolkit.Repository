namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;

    public class TestAdoNetConnectionStringFactory
    {
        public static Tuple<string, string> Create()
        {
            var provider = "System.Data.SqlServerCe.4.0";
            var currentFile = Path.GetTempFileName();
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";

            if (File.Exists(currentFile))
                File.Delete(currentFile);

            CreateDatabase(provider, connectionString);

            return Tuple.Create(provider, connectionString);
        }

        private static void CreateDatabase(string provider, string connectionString)
        {
            new SqlCeEngine(connectionString).CreateDatabase();

            var factory = DbProviderFactories.GetFactory(provider);
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE Customer (
                                            Id int IDENTITY PRIMARY KEY,
                                            Name nvarchar (100),
                                            AddressId int)";

                    var result = command.ExecuteNonQuery();
                }
            }
        }
    }
}
