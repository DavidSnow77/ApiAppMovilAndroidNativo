namespace Abarrotes.Models
{
    public class VistaDetalleVentaEntity
    {
        public string VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string Producto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; }
    }
}
