namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Microsoft.EntityFrameworkCore;

    public class TestEfCoreDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public System.Data.Entity.DbSet<Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithMultipleAddresses> CustomerWithMultipleAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithCompositeAddress> CustomersWithCompositeAddress { get; set; }
        public System.Data.Entity.DbSet<CustomerWithTwoCompositePrimaryKey> CustomersWithTwoCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithThreeCompositePrimaryKey> CustomersWithThreeCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithNoIdentity> CustomersWithNoIdentity { get; set; }

        public TestEfCoreDbContext(DbContextOptions options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerWithNoIdentity>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Address)
                .WithOne(x => x.Customer);

            modelBuilder.Entity<CustomerCompositeAddress>()
                .HasOne(x => x.Customer)
                .WithOne(x => x.Address);

            modelBuilder.Entity<CustomerAddressWithMultipleAddresses>()
                .HasOne<CustomerWithMultipleAddresses>(x => x.Customer)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<CustomerWithTwoCompositePrimaryKey>()
                .HasKey(x => new { x.Id1, x.Id2 });

            modelBuilder.Entity<CustomerWithThreeCompositePrimaryKey>()
                .HasKey(x => new { x.Id1, x.Id2, x.Id3 });

            modelBuilder.Entity<CustomerCompositeAddress>()
                .HasKey(x => new { x.Id, x.CustomerId });
        }
    }
}