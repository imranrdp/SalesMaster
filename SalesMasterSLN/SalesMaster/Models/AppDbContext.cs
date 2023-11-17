using Microsoft.EntityFrameworkCore;

namespace SalesMaster.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SalesMasterTable> SalesMasters { get; set; }
        public DbSet<SalesDetails> SalesDetails { get; set; }
    }
}
