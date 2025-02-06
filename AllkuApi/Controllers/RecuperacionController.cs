using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecuperacionController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public RecuperacionController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("solicitar-recuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitudRecuperacionDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var cmd = new SqlCommand("GenerarTokenRecuperacion", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@nombre_usuario", dto.NombreUsuario);
                    cmd.Parameters.AddWithValue("@correo", dto.Correo);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var token = reader["token"].ToString();
                        // Aquí normalmente enviarías el correo con el link
                        var resetLink = $"https://allkuapi.sytes.net/reset-password?token={token}";

                        // Por ahora, solo retornamos el link (en producción, esto se enviaría por correo)
                        return Ok(new
                        {
                            mensaje = "Link de recuperación generado con éxito",
                            link = resetLink
                        });
                    }

                    return NotFound("Usuario o correo no encontrado");
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var cmd = new SqlCommand("ActualizarContrasena", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@token", new Guid(dto.Token));
                    cmd.Parameters.AddWithValue("@nueva_contrasena", dto.NuevaContrasena);

                    await cmd.ExecuteNonQueryAsync();
                    return Ok("Contraseña actualizada con éxito");
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }

    public class SolicitudRecuperacionDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string NuevaContrasena { get; set; }
    }
}