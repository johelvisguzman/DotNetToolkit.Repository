namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Data.Common;
    using System.Data.Entity;

    public class TestDbContext : DbContext
    {
        public DbSet<Customer> TestCustomerEntities { get; set; }

        public TestDbContext(DbConnection connection) : base(connection, true)
        {
        }
    }
}
