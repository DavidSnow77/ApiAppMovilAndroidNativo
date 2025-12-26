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
    public class UsuarioController : ControllerBase
    {
        private readonly AbarrotesReyesContext _context;

        public UsuarioController(AbarrotesReyesContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> ListaUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();

                if (usuarios.Count == 0)
                {
                    return NotFound(new { Mensaje = "No hay usuarios registrados." });
                }

                return Ok(new { Mensaje = "Lista de usuarios", usuarios });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error al obtener los usuarios.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("getUsuario/{usuarioId}")]

        public async Task<ActionResult> ObtenerUsuario(string usuarioId)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
            if (usuario == null)
            {
                return NotFound(new { Mensaje = "Usuario no encontrado" });
            }
            return Ok(new { Mensaje = "Usuario encontrado", Usuario = usuario });
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginDTO.Correo);
               // .FirstOrDefaultAsync(u => u.Usuario == loginDTO.Usuario && u.Clave == loginDTO.clave);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Clave, usuario.Clave))
            {
                // Respuesta inmediata y clara si no existe
                return NotFound(new { Mensaje = "El correo no existe o las credenciales son incorrectas" });
            }

            return Ok(usuario);
        }

        /* [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == loginDTO.Usuario && u.Clave == loginDTO.clave);

            if (usuario == null)
            {
                // Respuesta inmediata y clara si no existe
                return NotFound(new { Mensaje = "El usuario no existe o las credenciales son incorrectas" });
            }

            var returnUsuario = new UsuarioDTO
            {
                Usuario = usuario.Usuario,
                Clave = usuario.Clave
            };

            return Ok(returnUsuario);
        } */

        [HttpPost]
        public async Task<ActionResult> GuardarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuarioExiste = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == usuarioDTO.Usuario);

            if (usuarioExiste != null)
            {
                return BadRequest(new { Mensaje = "El usuario ya existe." });
            }

            UsuarioEntity usuario = new UsuarioEntity()
            {
                Usuario = usuarioDTO.Usuario,
                Correo = usuarioDTO.Correo,
                Clave = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Clave)
            };

            await _context.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Usuario registrado", Usuario = usuario });
        }

        [HttpPut("putUsuarios/{usuarioId}")]

        public async Task<ActionResult> ActualizarUsuarios(string usuarioId, [FromBody] UsuarioUpdateDTO usuarioDTO)
        {
            var usuarionoexiste = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuarionoexiste == null)
            {

                return BadRequest(new { Mensaje = "No se puede actualizar el usuario no existe " });
            }

            usuarionoexiste.Usuario = usuarioDTO.Usuario;
            usuarionoexiste.Correo = usuarioDTO.Correo;
           // usuarionoexiste.Clave = usuarioDTO.Clave;

            if (!string.IsNullOrEmpty(usuarioDTO.Clave))
            {
                usuarionoexiste.Clave = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Clave);
            }

            _context.Usuarios.Update(usuarionoexiste);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Usuario actualizado", Usuario = usuarionoexiste });

        }

        [HttpDelete("deleteUsuarios/{usuarioId}")]
        public async Task<ActionResult> eliminarUsuarios(string usuarioId)

        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null)
            {
                return NotFound(new { Mensaje = "No se puede eliminar el usuario  ya que no existe" });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Usuario eliminado correctamente." });


        }


    }
}
