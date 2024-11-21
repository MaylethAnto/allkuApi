using AllkuApi.Data;
using AllkuApi.DataTransferObjects_DTO_;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SqlConnection _connection;
    private readonly AllkuDbContext _dbContext;
    private const string _claveMaestra = "TuClaveSecretaUnica2024$"; // La clave maestra que solo tú conoces

    public AuthController(IConfiguration configuration)
    {
        _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    // Endpoint para registrar un administrador
    [HttpPost("registrar-administrador")]
    public async Task<IActionResult> RegistrarAdministrador([FromBody] RegistrarAdministradorDto dto)
    {
        if (dto.ClaveMaestra != _claveMaestra)
        {
            return Unauthorized("Clave maestra incorrecta");
        }

        var cmd = new SqlCommand("RegistrarAdministrador", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@cedula_administrador", dto.CedulaAdministrador);
        cmd.Parameters.AddWithValue("@nombre_administrador", dto.NombreAdministrador);
        cmd.Parameters.AddWithValue("@usuario_administrador", dto.UsuarioAdministrador);
        cmd.Parameters.AddWithValue("@correo_administrador", dto.CorreoAdministrador);
        cmd.Parameters.AddWithValue("@contrasena_administrador", dto.ContrasenaAdministrador);
        cmd.Parameters.AddWithValue("@clave_maestra", dto.ClaveMaestra);

        try
        {
            _connection.Open();
            await cmd.ExecuteNonQueryAsync();
            _connection.Close();
            return Ok("Administrador registrado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al registrar el administrador: {ex.Message}");
        }
    }

    // Endpoint para registrar un usuario normal (dueño o paseador)
    [HttpPost("registrar-usuario")]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegistrarUsuarioDto dto)
    {
        var cmd = new SqlCommand("RegistrarUsuario", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@cedula", dto.Cedula);
        cmd.Parameters.AddWithValue("@nombre", dto.Nombre);
        cmd.Parameters.AddWithValue("@correo", dto.Correo);
        cmd.Parameters.AddWithValue("@usuario", dto.Usuario);
        cmd.Parameters.AddWithValue("@contrasena", dto.Contrasena);
        cmd.Parameters.AddWithValue("@rol", dto.Rol);

        try
        {
            _connection.Open();
            await cmd.ExecuteNonQueryAsync();
            _connection.Close();
            return Ok("Usuario registrado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al registrar el usuario: {ex.Message}");
        }


    }

    // Endpoint de login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Verificar las credenciales utilizando el método IniciarSesion
        var usuario = await _dbContext.IniciarSesion(request.NombreUsuario, request.Contrasena);

        if (usuario != null)
        {
            // Si las credenciales son correctas, devolver un mensaje de éxito
            return Ok(new { success = true, message = "Login exitoso", user = usuario });
        }
        else
        {
            // Si las credenciales son incorrectas, devolver un mensaje de error
            return BadRequest(new { success = false, message = "Credenciales incorrectas" });
        }
    }

    // Modelo para el login
    public class LoginRequest
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }
}
