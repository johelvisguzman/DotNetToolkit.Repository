namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.Data.Common;
    using System.Data.Entity;

    public class TestEfDbContext : System.Data.Entity.DbContext
    {
        public System.Data.Entity.DbSet<Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithMultipleAddresses> CustomerWithMultipleAddresses { get; set; }
        public System.Data.Entity.DbSet<CustomerWithCompositeAddress> CustomersWithCompositeAddress { get; set; }
        public System.Data.Entity.DbSet<CustomerWithTwoCompositePrimaryKey> CustomersWithTwoCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithThreeCompositePrimaryKey> CustomersWithThreeCompositePrimaryKey { get; set; }
        public System.Data.Entity.DbSet<CustomerWithNoIdentity> CustomersWithNoIdentity { get; set; }

        public TestEfDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasOptional(x => x.Address)
                .WithRequired(x => x.Customer);

            modelBuilder.Entity<CustomerCompositeAddress>()
                .HasRequired(x => x.Customer)
                .WithOptional(x => x.Address);

            modelBuilder.Entity<CustomerAddressWithMultipleAddresses>()
                .HasRequired<CustomerWithMultipleAddresses>(x => x.Customer)
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
