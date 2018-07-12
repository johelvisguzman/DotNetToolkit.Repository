namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using AdoNet;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.IO;

    public class TestAdoNetContextFactory
    {
        public static AdoNetRepositoryContext Create()
        {
            var currentFile = TestPathHelper.GetTempFileName();

            var provider = "System.Data.SqlServerCe.4.0";
            var connectionString = $"Data Source={currentFile};Persist Security Info=False";

            if (File.Exists(currentFile))
                File.Delete(currentFile);

            CreateDatabase(provider, connectionString);

            return new AdoNetRepositoryContext(provider, connectionString);
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
                    command.CommandText = @"CREATE TABLE Customers (
                                            Id int IDENTITY PRIMARY KEY,
                                            Name nvarchar (100),
                                            AddressId int)";

                    var result = command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomersWithNoIdentity (
                                            Id int PRIMARY KEY,
                                            Name nvarchar (100),
                                            AddressId int)";

                    var result = command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerWithTwoCompositePrimaryKeys (
                                            Id1 int,
                                            Id2 int,
                                            Name nvarchar (100),
                                            PRIMARY KEY (Id1, Id2))";

                    var result = command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerWithThreeCompositePrimaryKeys (
                                            Id1 int,
                                            Id2 int,
                                            Id3 int,
                                            Name nvarchar (100),
                                            PRIMARY KEY (Id1, Id2, Id3))";

                    var result = command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerCompositeAddresses (
                                            Id int,
                                            CustomerId int,
                                            Street nvarchar (100),
                                            City nvarchar (100),
                                            State nvarchar (2),
                                            PRIMARY KEY (Id, CustomerId))";

                    var result = command.ExecuteNonQuery();
                }

                using (var command = factory.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE CustomerAddresses (
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
