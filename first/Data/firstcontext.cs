using Microsoft.EntityFrameworkCore;
using first.module;

namespace first.Data
{
    public class firstcontext : DbContext
    {
        public DbSet<order> orders { get; set; }

        public firstcontext(DbContextOptions<firstcontext> options) : base(options)
        {
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=AH-LEGION\SQLEXPRESS; Initial Catalog=Orders;Trusted_Connection=True;TrustServerCertificate=True;"); 
        }*/
    }
}
