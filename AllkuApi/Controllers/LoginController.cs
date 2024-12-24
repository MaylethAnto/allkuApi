using AllkuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly AllkuDbContext _context;

        public LoginController(IConfiguration configuration, AllkuDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Contrasena))
                {
                    return BadRequest(new { mensaje = "Usuario y contraseña son requeridos" });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("IniciarSesion", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@nombre_usuario", request.NombreUsuario);
                    command.Parameters.AddWithValue("@contrasena", request.Contrasena); 

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var mensaje = reader["Mensaje"].ToString();

                            if (mensaje == "Login exitoso")
                            {
                                var rol = reader["rol_usuario"].ToString();
                                var esPrimeraVez = Convert.ToBoolean(reader["EsPrimeraVez"]);

                                return Ok(new
                                {
                                    mensaje = "Login exitoso",
                                    rol,
                                    esPrimeraVez
                                });
                            }

                            return Unauthorized(new { mensaje });
                        }

                        return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { mensaje = $"Error de base de datos: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // Método para comprobar si es la primera vez que el usuario con rol de "Dueño" inicia sesión
        [HttpGet("esprimavez")]
        public async Task<IActionResult> EsPrimeraVez([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { Message = "El nombre de usuario no puede estar vacío" });
                }

                var user = await _context.ManejoPerfiles
                    .AsNoTracking()
                    .Include(mp => mp.Dueno)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == username && u.RolUsuario == "Dueño");

                if (user == null)
                {
                    return NotFound(new { Message = $"Usuario '{username}' no encontrado o no es un dueño" });
                }

                var esPrimeraVez = !user.UltimoInicioSesion.HasValue;

                return Ok(new
                {
                    EsPrimeraVez = esPrimeraVez,
                    Username = username,
                    UltimoInicioSesion = user.UltimoInicioSesion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error interno del servidor",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        public class LoginRequest
        {
            public string NombreUsuario { get; set; }
            public string Contrasena { get; set; }
        }
    }
}