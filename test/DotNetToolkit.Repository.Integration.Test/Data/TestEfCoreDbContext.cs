namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Microsoft.EntityFrameworkCore;

    public class TestEfCoreDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerWithMultipleAddresses> CustomerWithMultipleAddresses { get; set; }
        public DbSet<CustomerWithCompositeAddress> CustomersWithCompositeAddress { get; set; }
        public DbSet<CustomerWithTwoCompositePrimaryKey> CustomersWithTwoCompositePrimaryKey { get; set; }
        public DbSet<CustomerWithThreeCompositePrimaryKey> CustomersWithThreeCompositePrimaryKey { get; set; }
        public DbSet<CustomerWithNoIdentity> CustomersWithNoIdentity { get; set; }

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