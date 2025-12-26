using Abarrotes.BaseDedatos;
using Abarrotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Abarrotes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasAgrupadasController : ControllerBase
    {
        private readonly AbarrotesReyesContext _context;

        public VentasAgrupadasController(AbarrotesReyesContext context)
        {
            _context = context;
        }

        // 🔹 ENDPOINT ORIGINAL - NO TOCAR (para que tu app siga funcionando)
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _context.VistaDetalleVentas.ToListAsync();
            if (lista.Count == 0)
                return NotFound(new { Mensaje = "No hay datos en la vista." });

            return Ok(new { Mensaje = "Vista consultada correctamente", Detalles = lista });
        }

        // 🔹 NUEVO ENDPOINT - VENTAS AGRUPADAS
        [HttpGet("agrupadas")]
        public async Task<IActionResult> ObtenerVentasAgrupadas()
        {
            var lista = await _context.VistaDetalleVentas.ToListAsync();
            if (lista.Count == 0)
                return NotFound(new { Mensaje = "No hay datos en la vista." });

            // Agrupar SOLO por VentaId (los que tienen el mismo ID se agrupan)
            var ventasAgrupadas = lista
                .GroupBy(v => v.VentaId) // Agrupa solo los que tienen el mismo VentaId
                .Select(grupo => new
                {
                    VentaId = grupo.Key,
                    FechaVenta = grupo.First().FechaVenta,
                    Total = grupo.First().Total,
                    Productos = grupo.Select(item => new
                    {
                        Producto = item.Producto,
                        Precio = item.Precio,
                        Cantidad = item.Cantidad,
                        Estado = item.Estado
                    }).ToList()
                })
                .OrderByDescending(v => v.FechaVenta)
                .ToList();

            return Ok(new
            {
                Mensaje = "Ventas agrupadas obtenidas correctamente",
                Detalles = ventasAgrupadas
            });
        }

        // 🔹 ENDPOINT ORIGINAL - NO TOCAR
        [HttpGet("porVenta/{ventaId}")]
        public async Task<IActionResult> FiltrarPorVenta(string ventaId)
        {
            var resultados = await _context.VistaDetalleVentas
                .Where(v => v.VentaId == ventaId)
                .ToListAsync();

            if (resultados.Count == 0)
                return NotFound(new { Mensaje = "No se encontraron registros para esa venta." });

            return Ok(new
            {
                Mensaje = "Resultados encontrados",
                Detalles = resultados
            });
        }
    }
}
