using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using doublePartnersBack.db;
using doublePartnersBack.Models;

namespace doublePartnersBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishListProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Consultar listado de productos deseados de un usuario
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetWishListByUser(string userId)
        {
            var wishList = await _context.WishlistItems
                .Where(w => w.UserId == userId) // Comparación correcta con string
                .Include(w => w.Producto) // Incluir los detalles del producto
                .Select(w => w.Producto) // Solo seleccionar el producto
                .ToListAsync();

            if (wishList == null || !wishList.Any())
            {
                return NotFound("No se encontraron productos deseados para este usuario.");
            }

            return Ok(wishList);
        }

        // Agregar un producto como "producto deseado"
        [HttpPost]
        public async Task<ActionResult<WishlistItem>> AddProductoToWishList(string userId, int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }

            var wishListProduct = new WishlistItem
            {
                UserId = userId,
                ProductoId = productoId,
                Producto = producto
            };

            _context.WishlistItems.Add(wishListProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishListByUser), new { userId = userId }, wishListProduct);
        }

        // Eliminar un producto deseado
        [HttpDelete("{userId}/{productoId}")]
        public async Task<IActionResult> RemoveProductoFromWishList(string userId, int productoId)
        {
            var wishListProduct = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductoId == productoId); // Comparación correcta con string

            if (wishListProduct == null)
            {
                return NotFound("El producto deseado no fue encontrado.");
            }

            _context.WishlistItems.Remove(wishListProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
