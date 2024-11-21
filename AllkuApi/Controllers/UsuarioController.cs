using AllkuApi.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AllkuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AutenticacionService _autenticacionService;

        public UsuarioController(AutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = await _autenticacionService.IniciarSesion(
                    request.NombreUsuario,
                    request.Contrasena
                );

                return Ok(new
                {
                    NombreUsuario = usuario.NombreUsuario,
                    RolUsuario = usuario.RolUsuario
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            try
            {
                var usuario = await _autenticacionService.RegistrarUsuario(
                    request.NombreUsuario,
                    request.Contrasena,
                    request.RolUsuario
                );

                return CreatedAtAction(nameof(Login), new { nombreUsuario = usuario.NombreUsuario });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }

    public class RegistroRequest
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string RolUsuario { get; set; }
    }
}
