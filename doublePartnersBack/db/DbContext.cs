using doublePartnersBack.Models;
using Microsoft.EntityFrameworkCore;

namespace doublePartnersBack.db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; } // Define tu tabla de productos
        public DbSet<WishlistItem> WishlistItems { get; set; } // Define tu tabla de deseos
    }

}
