namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Data.Common;
    using System.Data.Entity;

    public class TestEfDbContext : System.Data.Entity.DbContext
    {
        public System.Data.Entity.DbSet<Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithTwoCompositePrimaryKey> CustomersWithTwoCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithThreeCompositePrimaryKey> CustomersWithThreeCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithNoIdentity> CustomersWithNoIdentity { get; set; }

        public TestEfDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerAddress>()
                .HasKey(e => e.CustomerId);

            modelBuilder.Entity<CustomerWithNoIdentity>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Customer>()
                 .HasOptional(s => s.Address)
                 .WithRequired(ad => ad.Customer);

            modelBuilder.Entity<CustomerWithTwoCompositePrimaryKey>()
                .HasKey(e => new { e.Id1, e.Id2 });

            modelBuilder.Entity<CustomerWithThreeCompositePrimaryKey>()
                .HasKey(e => new { e.Id1, e.Id2, e.Id3 });
        }
    }
}
