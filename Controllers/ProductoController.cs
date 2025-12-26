using System.Threading.Tasks;
using Abarrotes.BaseDedatos;
using Abarrotes.DTO;
using Abarrotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Abarrotes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly AbarrotesReyesContext _context;

        public ProductoController(AbarrotesReyesContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> ListaProductos()
        {
            try
            {
                var productos = await _context.Productos.ToListAsync();

                if (productos.Count == 0)
                {
                    return NotFound(new { Mensaje = "No hay productos registrados." });
                }

                return Ok(new { Mensaje = "Lista de productos",  productos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error al obtener los productos.",
                    Error = ex.Message
                });
            }
        }
        [HttpGet("getProducto/{productoId}")]

        public async Task<ActionResult> ObtenerUsuario(string productoId)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(u => u.ProductoId == productoId);
            if (producto == null)
            {
                return NotFound(new { Mensaje = "Producto no encontrado" });
            }
            return Ok(new { Mensaje = "Producto encontrado", Producto = producto });
        }

        [HttpPost]
        public async Task<ActionResult> GuardarProducto([FromBody] ProductoDTO productoDTO)
        {
            if (string.IsNullOrWhiteSpace(productoDTO.NombreProducto)) { 
            
                return BadRequest(new { Mensaje = "El nombre del producto es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(productoDTO.CodigoProducto))
            {
                return BadRequest(new { Mensaje = "El código de producto es obligatorio." });
            }

            if (productoDTO.Cantidad <= 0 || productoDTO.Cantidad % 1 != 0)
            {
                return BadRequest("La cantidad debe ser un número entero mayor a cero.");
            }

            if (productoDTO.Precio <= 0 )
            {
                return BadRequest("El precio debe ser un número entero mayor a cero.");
            }

            var productoExiste = await _context.Productos
                .FirstOrDefaultAsync(p => p.NombreProducto == productoDTO.NombreProducto);

            if (productoExiste != null)
            {
                return BadRequest(new { Mensaje = "Ya existe un producto con ese nombre." });
            }

            var codigoExiste = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoProducto == productoDTO.CodigoProducto);

            if (codigoExiste != null)
            {
                return BadRequest(new { Mensaje = "Ya existe un producto con ese código de barras." });
            }

            ProductoEntity producto = new ProductoEntity()
            {
                NombreProducto = productoDTO.NombreProducto,
                CodigoProducto = productoDTO.CodigoProducto,
                Cantidad = productoDTO.Cantidad,
                Precio = productoDTO.Precio
            };

            await _context.AddAsync(producto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Producto registrado", Producto = producto });
        }

        [HttpPut("putProductos/{productoId}")]
        public async Task<ActionResult> ActualizarProductos(string productoId, [FromBody] ProductoDTO productoDTO)
        {
            // 1. Verificar si el producto existe
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProductoId == productoId);

            if (producto == null)
            {
                return BadRequest(new { Mensaje = "No se puede actualizar, el producto no existe." });

            }

            // 2. Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(productoDTO.NombreProducto))
            {
                return BadRequest(new { Mensaje = "El nombre del producto es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(productoDTO.CodigoProducto))
                return BadRequest(new { Mensaje = "El código de producto es obligatorio." });

            // 3. Validar duplicados (excluyendo el producto actual)
            var nombreDuplicado = await _context.Productos
                .FirstOrDefaultAsync(p => p.NombreProducto == productoDTO.NombreProducto && p.ProductoId != productoId);

            if (nombreDuplicado != null)
            {
                return BadRequest(new { Mensaje = "Ya existe un producto con el nombre que deseas actualizar." });

            }
            var codigoDuplicado = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoProducto == productoDTO.CodigoProducto && p.ProductoId != productoId);

            if (codigoDuplicado != null)
            {
                return BadRequest(new { Mensaje = "Ya existe un producto con ese código de barras." });

            }

            // 4. Actualizar campos
            producto.NombreProducto = productoDTO.NombreProducto;
            producto.CodigoProducto = productoDTO.CodigoProducto;
            producto.Cantidad = productoDTO.Cantidad;
            producto.Precio = productoDTO.Precio;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Producto actualizado", Producto = producto });
        }

        [HttpDelete("deleteproductos/{productoId}")]
        public async Task<ActionResult> eliminarProductos(string productoId)

        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProductoId == productoId);

            if (producto == null)
            {
                return NotFound(new { Mensaje = "No se puede eliminar el producto  ya que no existe" });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Producto eliminado correctamente." });


        }

        [HttpGet("getProductoPorCodigo/{codigo}")]
        public async Task<ActionResult> ObtenerProductoPorCodigo(string codigo)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoProducto == codigo);

            if (producto == null)
            {
                return NotFound(new { Mensaje = "Producto no encontrado por código." });
            }

            return Ok(new { Mensaje = "Producto encontrado", Producto = producto });
        }

    }
}
