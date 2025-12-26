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
    public class DetalleVentaController : ControllerBase
    {
        private readonly AbarrotesReyesContext _context;

        public DetalleVentaController(AbarrotesReyesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> ListaDetalleVenta()
        {
            try
            {
                var detalleVentas = await _context.DetalleVentas.ToListAsync();

                if (detalleVentas.Count == 0)
                {
                    return NotFound(new { Mensaje = "No hay Detalles registrados." });
                }

                return Ok(new { Mensaje = "Lista de detalles", detalleVentas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error al obtener los detalles.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("getDetalleVenta/{productoId}/{ventaId}")]
        public async Task<ActionResult> ObtenerDetalleVenta(string productoId, string ventaId)
        {
            var detalleVenta = await _context.DetalleVentas
                .FirstOrDefaultAsync(dv => dv.ProductoId == productoId && dv.VentaId == ventaId);

            if (detalleVenta == null)
            {
                return NotFound(new { Mensaje = "Detalle de venta no encontrado" });
            }

            return Ok(new { Mensaje = "Detalle de venta encontrado", DetalleVenta = detalleVenta });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarDetalleVenta([FromBody] DetalleVentaDTO detalleventaDTO)
        {
            // 🔒 Validaciones de campos obligatorios
            if (string.IsNullOrWhiteSpace(detalleventaDTO.ProductoId))
            {
                return BadRequest("El campo Producto es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(detalleventaDTO.VentaId))
            {
                return BadRequest("El campo Venta es obligatorio.");
            }

            if (detalleventaDTO.Cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a cero.");
            }

            if (string.IsNullOrWhiteSpace(detalleventaDTO.Estado))
            {
                return BadRequest("El campo Estado es obligatorio.");
            }

            //  Validar que el estado sea válido
            var estadosValidos = new[] { "Completada", "Cancelada" };
            if (!estadosValidos.Contains(detalleventaDTO.Estado, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("El estado debe ser 'Completada' o 'Cancelada'.");
            }

            //  Validar existencia de Producto y Venta
            var producto = await _context.Productos.FindAsync(detalleventaDTO.ProductoId);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }

            var venta = await _context.Ventas.FindAsync(detalleventaDTO.VentaId);
            if (venta == null)
            {
                return NotFound("Venta no encontrada.");
            }

            //  Validar que no exista ya el detalle
            var existeDetalle = await _context.DetalleVentas
                .AnyAsync(d => d.ProductoId == detalleventaDTO.ProductoId && d.VentaId == detalleventaDTO.VentaId);

            if (existeDetalle)
            {
                return Conflict("Ya existe un detalle para este producto en esta venta.");
            }

            //  Crear nuevo detalle
            var estadoNormalizado = detalleventaDTO.Estado.Trim().ToLower() == "completada" ? "Completada" : "Cancelada";

            var detalle = new DetalleVentaEntity
            {
                ProductoId = detalleventaDTO.ProductoId,
                VentaId = detalleventaDTO.VentaId,
                Cantidad = detalleventaDTO.Cantidad,
                Estado = estadoNormalizado
            };

            await _context.DetalleVentas.AddAsync(detalle);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Detalle de venta registrado", Detalle = detalle });
        }

        [HttpPut("PutdetalleVentas/{productoId}/{ventaId}")]
        public async Task<IActionResult> ActualizarDetalleVenta(string productoId, string ventaId, [FromBody] DetalleEmergenciaDTO detalleventadto)
        {
           
            var detalleExistente = await _context.DetalleVentas
                .FirstOrDefaultAsync(d => d.ProductoId == productoId && d.VentaId == ventaId);

            if (detalleExistente == null)
            {
                return NotFound("No se encontró el detalle de venta con la combinación especificada.");
            }
            if (detalleventadto.Cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a cero.");
            }

            if (string.IsNullOrWhiteSpace(detalleventadto.Estado))
            {
                return BadRequest("El campo Estado es obligatorio.");

            }

            var estadosValidos = new[] { "Completada", "Cancelada" };
            if (!estadosValidos.Contains(detalleventadto.Estado, StringComparer.OrdinalIgnoreCase))
                return BadRequest("El estado debe ser 'Completada' o 'Cancelada'.");

           
            detalleExistente.Cantidad = detalleventadto.Cantidad;
            detalleExistente.Estado = detalleventadto.Estado.Trim().ToLower() == "completada" ? "Completada" : "Cancelada";

            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Detalle de venta actualizado", Detalle = detalleExistente });
        }

        [HttpDelete("deletedetalleVenta/{productoId}/{ventaId}")]
        public async Task<IActionResult> EliminarDetalleVenta(string productoId, string ventaId)
        {
            

            var detalle = await _context.DetalleVentas
                .FirstOrDefaultAsync(d => d.ProductoId == productoId && d.VentaId == ventaId);

            if (detalle == null)
            {
                return NotFound(new { Mensaje = "No se puede eliminar el detalle de venta porque no existe." });
            }

            _context.DetalleVentas.Remove(detalle);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Detalle de venta eliminado correctamente." });
        }



    }
}