using System.Text.Json.Serialization;

namespace Abarrotes.Models
{
    public class ProductoEntity
    {
        public string ProductoId { get; set; }

        public string NombreProducto { get; set; }

        public string CodigoProducto { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

         [JsonIgnore]
         public ICollection<DetalleVentaEntity> Detalles { get; set; } = new List<DetalleVentaEntity>();
    }
}
