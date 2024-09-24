using doublePartnersBack.db;
using doublePartnersBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    }
}
