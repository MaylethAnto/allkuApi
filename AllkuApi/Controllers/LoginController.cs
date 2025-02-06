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
        [Consumes("application/json", "application/x-www-form-urlencoded")] // Acepta tanto JSON como form-urlencoded
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Si los datos vienen como form-urlencoded, intentar obtenerlos del FormData
                if (request == null && Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    request = new LoginRequest
                    {
                        NombreUsuario = form["NombreUsuario"],
                        Contrasena = form["Contrasena"]
                    };
                }

                if (request == null || string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Contrasena))
                {
                    return BadRequest(new LoginResponse
                    {
                        Mensaje = "Usuario y contraseña son requeridos",
                        Exitoso = false
                    });
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
                                var response = new LoginResponse
                                {
                                    Mensaje = "Login exitoso",
                                    Exitoso = true,
                                    Rol = reader["rol_usuario"].ToString(),
                                    EsPrimeraVez = Convert.ToBoolean(reader["EsPrimeraVez"]),
                                    CedulaDueno = reader["cedula_dueno"] != DBNull.Value ? reader["cedula_dueno"].ToString() : null,
                                    CedulaPaseador = reader["cedula_paseador"] != DBNull.Value ? reader["cedula_paseador"].ToString() : null,
                                    CelularPaseador = reader["celular_paseador"] != DBNull.Value ? reader["celular_paseador"].ToString() : null,
                                    NombrePaseador = reader["nombre_paseador"] != DBNull.Value ? reader["nombre_paseador"].ToString() : null
                                };

                                // Actualizar último inicio de sesión si es necesario
                                await ActualizarUltimoInicioSesion(request.NombreUsuario);

                                return Ok(response);
                            }

                            return Unauthorized(new LoginResponse
                            {
                                Mensaje = mensaje,
                                Exitoso = false
                            });
                        }

                        return Unauthorized(new LoginResponse
                        {
                            Mensaje = "Usuario o contraseña incorrectos",
                            Exitoso = false
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new LoginResponse
                {
                    Mensaje = $"Error de base de datos: {ex.Message}",
                    Exitoso = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginResponse
                {
                    Mensaje = $"Error interno del servidor: {ex.Message}",
                    Exitoso = false
                });
            }
        }

        private async Task ActualizarUltimoInicioSesion(string nombreUsuario)
        {
            var usuario = await _context.ManejoPerfiles.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
            if (usuario != null)
            {
                usuario.UltimoInicioSesion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        [HttpGet("esprimavez")]
        public async Task<ActionResult<PrimeraVezResponse>> EsPrimeraVez([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new PrimeraVezResponse
                    {
                        Mensaje = "El nombre de usuario no puede estar vacío",
                        Exitoso = false
                    });
                }

                var user = await _context.ManejoPerfiles
                    .AsNoTracking()
                    .Include(mp => mp.Dueno)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == username && u.RolUsuario == "Dueño");

                if (user == null)
                {
                    return NotFound(new PrimeraVezResponse
                    {
                        Mensaje = $"Usuario '{username}' no encontrado o no es un dueño",
                        Exitoso = false
                    });
                }

                return Ok(new PrimeraVezResponse
                {
                    Mensaje = "Consulta exitosa",
                    Exitoso = true,
                    EsPrimeraVez = !user.UltimoInicioSesion.HasValue,
                    Username = username,
                    UltimoInicioSesion = user.UltimoInicioSesion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PrimeraVezResponse
                {
                    Mensaje = "Error interno del servidor",
                    Exitoso = false,
                    Error = ex.Message
                });
            }
        }

        // Clases de modelo
        public class LoginRequest
        {
            public string NombreUsuario { get; set; }
            public string Contrasena { get; set; }
        }

        public class LoginResponse
        {
            public string Mensaje { get; set; }
            public bool Exitoso { get; set; }
            public string Rol { get; set; }
            public bool EsPrimeraVez { get; set; }
            public string CedulaDueno { get; set; }
            public string CedulaPaseador { get; set; }
            public string CelularPaseador { get; set; }
            public string NombrePaseador { get; set; }
        }

        public class PrimeraVezResponse
        {
            public string Mensaje { get; set; }
            public bool Exitoso { get; set; }
            public bool EsPrimeraVez { get; set; }
            public string Username { get; set; }
            public DateTime? UltimoInicioSesion { get; set; }
            public string Error { get; set; }
        }
    }
}