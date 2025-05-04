using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.Models
{
    public class HirafeyatContext:IdentityDbContext<ApplicationUser>
    {
        public HirafeyatContext():base()
        {
            
        }
        public HirafeyatContext(DbContextOptions<HirafeyatContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
}
