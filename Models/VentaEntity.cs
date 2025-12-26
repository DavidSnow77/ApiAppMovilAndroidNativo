using System.Text.Json.Serialization;

namespace Abarrotes.Models
{
    public class VentaEntity
    {
        public string VentaId { get; set; }

        public DateTime FechaVenta { get; set; }

        public decimal Total { get; set; }

        [JsonIgnore]
        public ICollection<DetalleVentaEntity> Detalles { get; set; } = new List<DetalleVentaEntity>();

    }
}
