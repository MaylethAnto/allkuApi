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

    //GET administrador
    // Endpoint para obtener todos los administradores o un administrador específico por su ID
    [HttpGet("administradores/{cedula_administrador?}")]
    public async Task<IActionResult> ObtenerAdministradores(int? cedula_administrador)
    {
        var cmd = new SqlCommand("ObtenerAdministradores", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (cedula_administrador.HasValue)
        {
            cmd.Parameters.AddWithValue("@cedula_administrador", cedula_administrador.Value);
        }

        try
        {
            _connection.Open();
            var reader = await cmd.ExecuteReaderAsync();
            var administradores = new List<dynamic>();

            while (reader.Read())
            {
                administradores.Add(new
                {
                    cedula_administrador = reader["cedula_administrador"],
                    nombre_administrador = reader["Nombre"],
                    usuario_administrador = reader["usuario_administrador"],
                    correo_administrador = reader["correo_administrador"],
                    contrasena_administrador = reader["contrasena_administrador"]
                });
            }

            _connection.Close();
            return Ok(administradores);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener administradores: {ex.Message}");
        }
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

    //Endpoint pull administrador

    // Endpoint para actualizar un administrador
    [HttpPut("actualizar-administrador/{cedula_administrador}")]
    public async Task<IActionResult> ActualizarAdministrador(int cedula, [FromBody] RegistrarAdministradorDto dto)
    {
        if (dto.ClaveMaestra != _claveMaestra)
        {
            return Unauthorized("Clave maestra incorrecta");
        }

        var cmd = new SqlCommand("ActualizarAdministrador", _connection)
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
            return Ok("Administrador actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar el administrador: {ex.Message}");
        }
    }

    //Endpont delete admin
    [HttpDelete("eliminar-administrador/{cedula_administrador}")]
    public async Task<IActionResult> EliminarAdministrador(int cedula_administrador, [FromQuery] string claveMaestra)
    {
        if (claveMaestra != _claveMaestra)
        {
            return Unauthorized("Clave maestra incorrecta");
        }

        var cmd = new SqlCommand("EliminarAdministrador", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@cedula", cedula_administrador);

        try
        {
            _connection.Open();
            await cmd.ExecuteNonQueryAsync();
            _connection.Close();
            return Ok("Administrador eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar el administrador: {ex.Message}");
        }
    }



    //Endpoint get usuario normal (dueño o paseador)
    [HttpGet("usuarios/{Cedula?}")]
    public async Task<IActionResult> ObtenerUsuarios(int? Cedula)
    {
        var cmd = new SqlCommand("ObtenerUsuarios", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (Cedula.HasValue)
        {
            cmd.Parameters.AddWithValue("@cedula_usuario", Cedula.Value);
        }

        try
        {
            _connection.Open();
            var reader = await cmd.ExecuteReaderAsync();
            var usuarios = new List<dynamic>();

            while (reader.Read())
            {
                usuarios.Add(new
                {
                    Cedula = reader["Cedula"],
                    Nombre = reader["Nombre"],
                    Correo = reader["Correo"],
                    Usuario = reader["Usuario"],
                    Rol = reader["Rol"],
                    Direccion = reader["Direccion"]
                });
            }

            _connection.Close();
            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener usuarios: {ex.Message}");
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

        cmd.Parameters.AddWithValue("@cedula_usuario", dto.Cedula);  
        cmd.Parameters.AddWithValue("@nombre_usuario", dto.Nombre);
        cmd.Parameters.AddWithValue("@correo_usuario", dto.Correo);
        cmd.Parameters.AddWithValue("@usuario_usuario", dto.Usuario);
        cmd.Parameters.AddWithValue("@contrasena_usuario", dto.Contrasena);
        cmd.Parameters.AddWithValue("@rol_usuario", dto.Rol);
        cmd.Parameters.AddWithValue("@direccion_usuario", dto.Direccion);
        cmd.Parameters.AddWithValue("@celular_usuario", dto.Celular);

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


    //Actualizar usuario normal (dueño o paseador)
    [HttpPut("actualizar-usuario/{Cedula}")]
    public async Task<IActionResult> ActualizarUsuario(int Cedula, [FromBody] RegistrarUsuarioDto dto)
    {
        var cmd = new SqlCommand("ActualizarUsuario", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@cedula_usuario", dto.Cedula);
        cmd.Parameters.AddWithValue("@nombre_usuario", dto.Nombre);
        cmd.Parameters.AddWithValue("@correo_usuario", dto.Correo);
        cmd.Parameters.AddWithValue("@usuario_usuario", dto.Usuario);
        cmd.Parameters.AddWithValue("@contrasena_usuario", dto.Contrasena);
        cmd.Parameters.AddWithValue("@rol_usuario", dto.Rol);
        cmd.Parameters.AddWithValue("@direccion_usuario", dto.Direccion);
        cmd.Parameters.AddWithValue("@celular_usuario", dto.Celular);

        try
        {
            _connection.Open();
            await cmd.ExecuteNonQueryAsync();
            _connection.Close();
            return Ok("Usuario actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar el usuario: {ex.Message}");
        }
    }

    //eliminar usuario normal (dueño o paseador)

    [HttpDelete("eliminar-usuario/{Cedula}")]
    public async Task<IActionResult> EliminarUsuario(int Cedula)
    {
        var cmd = new SqlCommand("EliminarUsuario", _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@cedula_usuario", Cedula);

        try
        {
            _connection.Open();
            await cmd.ExecuteNonQueryAsync();
            _connection.Close();
            return Ok("Usuario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar el usuario: {ex.Message}");
        }
    }



    // Modelo para el login
    public class LoginRequest
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }
}
