using doublePartnersBack.db;
using doublePartnersBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Necesario para ILogger
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doublePartnersBack.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Producto>> GetProductos()
        {
            _logger.LogInformation("Fetching all products from database");

            try
            {
                var productos = _context.Productos.ToList();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> AddProducto(Producto producto)
        {
            _logger.LogInformation("Adding a new product to the database");

            try
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product added successfully with ID: {Id}", producto.Id);
                return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new product.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                _logger.LogWarning("Mismatch between URL product ID and body product ID");
                return BadRequest("The product ID in the URL and the body do not match.");
            }

            _logger.LogInformation("Updating product with ID: {Id}", id);

            var existingProducto = await _context.Productos.FindAsync(id);
            if (existingProducto == null)
            {
                _logger.LogWarning("Product with ID: {Id} not found for update", id);
                return NotFound("Product not found.");
            }

            // Actualizar propiedades del producto existente
            existingProducto.Title = producto.Title;
            // Si hay más propiedades, puedes actualizarlas aquí

            _context.Entry(existingProducto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with ID: {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Productos.Any(p => p.Id == id))
                {
                    _logger.LogWarning("Product with ID: {Id} no longer exists in the database", id);
                    return NotFound("Product not found.");
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error while updating product with ID: {Id}", id);
                    return StatusCode(500, "Internal server error during product update.");
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            _logger.LogInformation("Deleting product with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                _logger.LogWarning("Product with ID: {Id} not found for deletion", id);
                return NotFound("Product not found.");
            }

            try
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with ID: {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product with ID: {Id}", id);
                return StatusCode(500, "Internal server error during product deletion.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoById(int id)
        {
            _logger.LogInformation("Fetching product with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                _logger.LogWarning("Product with ID: {Id} not found", id);
                return NotFound("Product not found.");
            }

            _logger.LogInformation("Product with ID: {Id} fetched successfully", id);
            return producto;
        }
    }
}
