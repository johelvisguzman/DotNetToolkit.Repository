namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Microsoft.EntityFrameworkCore;

    public class TestEfCoreDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public System.Data.Entity.DbSet<Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithTwoCompositePrimaryKey> CustomersWithTwoCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithThreeCompositePrimaryKey> CustomersWithThreeCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithNoIdentity> CustomersWithNoIdentity { get; set; }

        public TestEfCoreDbContext(DbContextOptions options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerAddress>()
                .HasKey(e => e.CustomerId);

            modelBuilder.Entity<CustomerWithNoIdentity>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Customer>()
                .HasOne(s => s.Address)
                .WithOne(ad => ad.Customer)
                .HasForeignKey<Customer>(x => x.AddressId);

            modelBuilder.Entity<CustomerWithTwoCompositePrimaryKey>()
                .HasKey(e => new { e.Id1, e.Id2 });

            modelBuilder.Entity<CustomerWithThreeCompositePrimaryKey>()
                .HasKey(e => new { e.Id1, e.Id2, e.Id3 });
        }
    }
}