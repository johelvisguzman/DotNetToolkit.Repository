namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;

    public class TestAdoNetConnectionStringFactory
    {
        public static void Create(out string provider, out string connectionString)
        {
            var currentFile = Path.GetTempFileName();

            provider = "System.Data.SqlServerCe.4.0";
            connectionString = $"Data Source={currentFile};Persist Security Info=False";

            if (File.Exists(currentFile))
                File.Delete(currentFile);

            CreateDatabase(provider, connectionString);
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

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerAddress (
                                            Id int IDENTITY PRIMARY KEY,
                                            Street nvarchar (100),
                                            City nvarchar (100),
                                            State nvarchar (2),
                                            CustomerId int)";

                    var result = command.ExecuteNonQuery();
                }
            }
        }
    }
}
