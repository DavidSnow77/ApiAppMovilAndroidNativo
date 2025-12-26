using System.Text.Json.Serialization;
using Abarrotes.Models;
namespace Abarrotes.DTO
{
        public class DetalleVentaDTO
      {

          public string ProductoId { get; set; }

           public string VentaId { get; set; }

            public int Cantidad { get; set; }

           public string Estado { get; set; }
      }
   }

