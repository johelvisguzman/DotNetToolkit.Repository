#if NETFULL
namespace DotNetToolkit.Repository.Performance.Data
{
    using System.Data.Common;
    using System.Data.Entity;

    public class EfDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public EfDbContext(DbConnection connection, bool contextOwnsConnection) : base(connection, contextOwnsConnection) { }
    }
}
#endif