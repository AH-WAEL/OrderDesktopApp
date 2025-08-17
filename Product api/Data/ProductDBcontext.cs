using Microsoft.EntityFrameworkCore;
using productapi;

namespace Product_api.Data
{
    public class ProductDBcontext(DbContextOptions<ProductDBcontext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: only return active products globally
            modelBuilder.Entity<Product>().HasQueryFilter(o => o.IsActive);
        }
    }
}
