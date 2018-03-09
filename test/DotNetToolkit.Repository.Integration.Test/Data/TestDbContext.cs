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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                 .HasOptional(s => s.Address)
                 .WithRequired(ad => ad.Customer);
        }
    }
}
