namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Running;
    using System;
#if NETFULL
    using System.Data.SqlClient; 
#endif

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("// * Database Setup: Start *");
            EnsureDBSetup();
            Console.WriteLine("// * Database Setup: End *");

            Console.WriteLine("// * AzureStorageEmulator: Start *");
            Running.AzureStorageEmulatorManager.Start();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new Config());

            Running.AzureStorageEmulatorManager.Stop();
            Console.WriteLine("// * AzureStorageEmulator: End *");
        }

        private static void EnsureDBSetup()
        {
#if NETFULL
            using (var cnn = new SqlConnection(BenchmarkBase.ConnectionString))
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = @"
If (Object_Id('Customers') IS NOT NULL)
    DROP TABLE Customers;

CREATE TABLE Customers
(
	Id int primary key, 
	Name varchar(max),
    PartitionKey nvarchar(max),
    RowKey nvarchar(max)
);
";
                cmd.Connection = cnn;
                cmd.ExecuteNonQuery();
            } 
#endif
        }
    }
}
