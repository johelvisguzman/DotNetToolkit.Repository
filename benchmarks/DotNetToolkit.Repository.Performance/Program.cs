namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Running;
    using System;
    using System.Data.SqlClient;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("// * Database Setup: Start *");
            EnsureDBSetup();
            Console.WriteLine("// * Database Setup: End *");

            Console.WriteLine("// * AzureStorageEmulator: Start *");
            Tasks.AzureEmulatorTasks.Run();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new Config());

            Tasks.AzureEmulatorTasks.Cleanup();
            Console.WriteLine("// * AzureStorageEmulator: End *");
        }

        private static void EnsureDBSetup()
        {
            using (var cnn = new SqlConnection(BenchmarkBase.ConnectionString))
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = @"
If (Object_Id('Customers') Is Null)
Begin
	Create Table Customers
	(
		Id int identity primary key, 
		Name varchar(max)
	);
End
Else
Begin
    Truncate Table Customers;
End
";
                cmd.Connection = cnn;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
