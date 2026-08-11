using AuthWebapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthWebapi.Data
{
    public class userDbcontext(DbContextOptions<userDbcontext> options ) : DbContext(options)
    {
        public DbSet<users> users { get; set; }

    }
}
