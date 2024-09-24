using doublePartnersBack.Models;

namespace doublePartnersBack.db
{
    public static class DatabaseSeeder
    {
        public static void SeedData(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!context.Productos.Any())
            {
                var productos = new List<Producto>
            {
                new Producto { Id = 1, Title = "Producto 1" },
                new Producto { Id = 2, Title = "Producto 2" },
                new Producto { Id = 3, Title = "Producto 3" }
            };

                context.Productos.AddRange(productos);
                context.SaveChanges();
            }

            if (!context.WishlistItems.Any())
            {
                var wishlist = new List<WishlistItem>
            {
                new WishlistItem { Id = 1, ProductoId = 1, UserId = "user1" },
                new WishlistItem { Id = 2, ProductoId = 2, UserId = "user1" },
                new WishlistItem { Id = 3, ProductoId = 3, UserId = "user2" }
            };

                context.WishlistItems.AddRange(wishlist);
                context.SaveChanges();
            }
        }
    }

}
