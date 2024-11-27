using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("IniciarSesion", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Añadir los parámetros al procedimiento almacenado
                command.Parameters.AddWithValue("@nombre_usuario", request.NombreUsuario);
                command.Parameters.AddWithValue("@contrasena", request.Contrasena);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                // Leer el mensaje
                                var mensaje = reader["Mensaje"].ToString();  // Ahora "Mensaje" está disponible
                                if (mensaje == "Login exitoso")
                                {
                                    var rol = reader["rol_usuario"].ToString();
                                    return Ok(new
                                    {
                                        mensaje = "Login exitoso",
                                        rol = rol
                                    });
                                }
                                else
                                {
                                    return Unauthorized(mensaje); // Mostrar el mensaje de error
                                }
                            }
                        }
                        return Unauthorized("Usuario o contraseña incorrectos");
                    }
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, $"Error: {ex.Message}");
                }
            }
        }


    }
        

        public class LoginRequest
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }

}
