using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using doublePartnersBack.db;
using doublePartnersBack.Models;
using Microsoft.Extensions.Logging;

namespace doublePartnersBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WishListProductsController> _logger;

        public WishListProductsController(AppDbContext context, ILogger<WishListProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Consultar listado de productos deseados de un usuario
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetWishListByUser(string userId)
        {
            try
            {
                var wishList = await _context.WishlistItems
                    .Where(w => w.UserId == userId)
                    .Include(w => w.Producto)
                    .Select(w => w.Producto)
                    .ToListAsync();

                if (wishList == null || !wishList.Any())
                {
                    _logger.LogWarning("No se encontraron productos deseados para el usuario con ID: {UserId}", userId);
                    return NotFound("No se encontraron productos deseados para este usuario.");
                }

                return Ok(wishList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos deseados para el usuario con ID: {UserId}", userId);
                return StatusCode(500, "Se produjo un error en el servidor.");
            }
        }

        // Agregar un producto como "producto deseado"
        [HttpPost]
        public async Task<ActionResult<WishlistItem>> AddProductoToWishList(string userId, int productoId)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null)
                {
                    _logger.LogWarning("Producto no encontrado con ID: {ProductoId}", productoId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el producto a la lista de deseos del usuario con ID: {UserId}", userId);
                return StatusCode(500, "Se produjo un error en el servidor.");
            }
        }

        // Eliminar un producto deseado
        [HttpDelete("{userId}/{productoId}")]
        public async Task<IActionResult> RemoveProductoFromWishList(string userId, int productoId)
        {
            try
            {
                var wishListProduct = await _context.WishlistItems
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductoId == productoId);

                if (wishListProduct == null)
                {
                    _logger.LogWarning("El producto deseado no fue encontrado para el usuario con ID: {UserId} y producto con ID: {ProductoId}", userId, productoId);
                    return NotFound("El producto deseado no fue encontrado.");
                }

                _context.WishlistItems.Remove(wishListProduct);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto de la lista de deseos del usuario con ID: {UserId} y producto con ID: {ProductoId}", userId, productoId);
                return StatusCode(500, "Se produjo un error en el servidor.");
            }
        }
    }
}
