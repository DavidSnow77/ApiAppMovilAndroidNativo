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
    public class VentaController : ControllerBase
    {

        private readonly AbarrotesReyesContext _context;

        public VentaController(AbarrotesReyesContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult> ListaVentas()
        {
            try
            {
                var ventas = await _context.Ventas.ToListAsync();

                if (ventas.Count == 0)
                {
                    return NotFound(new { Mensaje = "No hay ventas registradas." });
                }

                return Ok(new { Mensaje = "Lista de Ventas", ventas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error al obtener las ventas.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("getVentas/{ventaId}")]

        public async Task<ActionResult> ObtenerUsuario(string ventaId)
        {
            var venta = await _context.Ventas.FirstOrDefaultAsync(u => u.VentaId == ventaId);
            if (venta == null)
            {
                return NotFound(new { Mensaje = "Venta no encontrada" });
            }
            return Ok(new { Mensaje = "Venta encontrada", Venta = venta  });
        }



        [HttpPost]
        public async Task<ActionResult> RegistrarVenta([FromBody] VentaDTO ventaDTO)
        {
            // Validación opcional: evitar fechas futuras
            if (ventaDTO.FechaVenta > DateTime.UtcNow)
                return BadRequest(new { Mensaje = "La fecha de venta no puede ser en el futuro." });

            if (ventaDTO.Total <= 0)
                return BadRequest("El total debe ser mayor a cero.");

            // Asegurar que la fecha tenga Kind = Utc
            DateTime fechaVenta = ventaDTO.FechaVenta;

            if (fechaVenta.Kind == DateTimeKind.Unspecified)
                fechaVenta = DateTime.SpecifyKind(fechaVenta, DateTimeKind.Utc);
            else if (fechaVenta.Kind == DateTimeKind.Local)
                fechaVenta = fechaVenta.ToUniversalTime();

            VentaEntity venta = new VentaEntity
            {
                FechaVenta = fechaVenta,
                Total = ventaDTO.Total
            };

            await _context.Ventas.AddAsync(venta);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Venta registrada", Venta = venta });
        }

        [HttpPut("putVentas/{ventaid}")]
        public async Task<IActionResult> ActualizarVenta(string ventaid, [FromBody] VentaDTO ventaDTO)
        {
            // Validación de fecha
            if (ventaDTO.FechaVenta > DateTime.UtcNow)
                return BadRequest("La fecha de la venta no puede estar en el futuro.");

            // Validación de total
            if (ventaDTO.Total <= 0)
                return BadRequest("El total debe ser mayor a cero.");

            // Buscar la venta
            var venta = await _context.Ventas.FindAsync(ventaid);
            if (venta == null)
                return NotFound("La venta no existe.");

            // Actualizar campos
            venta.FechaVenta = ventaDTO.FechaVenta;
            venta.Total = ventaDTO.Total;

            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Venta actualizada", Venta =venta });
        }


        [HttpDelete("deleteVentas/{ventaId}")]
        public async Task<ActionResult> eliminarUsuarios(string ventaId)

        {
            var venta = await _context.Ventas.FirstOrDefaultAsync(u => u.VentaId == ventaId);

            if (venta == null)
            {
                return NotFound(new { Mensaje = "No se puede eliminar la venta ya que no existe" });
            }

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Venta eliminada correctamente." });


        }






    }
}