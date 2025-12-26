using System.Text.Json.Serialization;

namespace Abarrotes.Models
{
    public class DetalleVentaEntity
    {
        public string ProductoId { get; set; }

        [JsonIgnore]
        public ProductoEntity Producto { get; set; }


        public string VentaId { get; set; }

        [JsonIgnore]
        public VentaEntity Venta { get; set; }

        public int Cantidad { get; set; }

        public string Estado { get; set; }

    }
   
    }

