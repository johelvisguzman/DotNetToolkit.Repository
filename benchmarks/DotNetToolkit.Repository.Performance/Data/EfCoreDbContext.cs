namespace DotNetToolkit.Repository.Performance.Data
{
    using Microsoft.EntityFrameworkCore;

    public class EfCoreDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public EfCoreDbContext(DbContextOptions options) : base(options) { }
    }
}
