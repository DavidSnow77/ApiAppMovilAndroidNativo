using System.Text.Json.Serialization;

namespace Abarrotes.DTO
{
    public class UsuarioDTO
    {
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
    }

    public class UsuarioUpdateDTO
    {
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string? Clave { get; set; }
    }

    public class LoginDTO
    {
        [JsonPropertyName("correo")]
        public string Correo { get; set; }

        [JsonPropertyName("clave")]
        public string Clave { get; set; }
    }
}
