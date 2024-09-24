using doublePartnersBack.db;
using doublePartnersBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doublePartnersBack.Controllers
{
    [Route("api/products")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Producto>> GetProductos()
        {
            return _context.Productos.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> AddProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest(); // El id de la URL y el id del cuerpo de la solicitud no coinciden
            }

            var existingProducto = await _context.Productos.FindAsync(id);
            if (existingProducto == null)
            {
                return NotFound();
            }

            // Actualizar propiedades del producto existente con los datos proporcionados
            existingProducto.Title = producto.Title;
            // Si hay más propiedades, puedes actualizarlas aquí

            _context.Entry(existingProducto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // La actualización fue exitosa, pero no devolvemos contenido
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent(); // Se devuelve NoContent cuando la eliminación fue exitosa
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoById(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(); // Si no se encuentra el producto, devolvemos un 404
            }

            return producto; // Devolvemos el producto encontrado (detalle del producto)
        }



    }
}
