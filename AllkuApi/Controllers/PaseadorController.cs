using AllkuApi.Data;
using AllkuApi.DataTransferObjects_DTO_;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaseadorController : ControllerBase
    {
        private readonly AllkuDbContext _context;
        private readonly string _connectionString;


        public PaseadorController(AllkuDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet("CaninosConDuenos")]
        public async Task<ActionResult<IEnumerable<CaninoConDuenoDto>>> GetCaninosConDuenos()
        {
            var caninos = await _context.Canino
                .Include(c => c.CedulaDueno)  // Incluimos la relación con Dueno
                .Select(c => new CaninoConDuenoDto
                {
                    IdCanino = c.IdCanino,
                    NombreCanino = c.NombreCanino,
                    RazaCanino = c.RazaCanino,
                    EdadCanino = c.EdadCanino,
                    PesoCanino = c.PesoCanino,
                    NombreDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.NombreDueno).FirstOrDefault(),
                    DireccionDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.DireccionDueno).FirstOrDefault(),
                    CelularDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.CelularDueno).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(caninos);
        }


        // GET: api/paseador/disponibles
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetPaseadoresDisponibles()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("ObtenerPaseadoresDisponibles", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var reader = await command.ExecuteReaderAsync();
                    var paseadores = new List<PaseadorDto>();

                    // Log column names for debugging
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"Column {i}: {reader.GetName(i)}");
                    }

                    while (await reader.ReadAsync())
                    {
                        paseadores.Add(new PaseadorDto
                        {
                            CedulaPaseador = reader.GetString(reader.GetOrdinal("cedula_paseador")),
                            NombrePaseador = reader.GetString(reader.GetOrdinal("nombre_paseador")),
                            CelularPaseador = reader.GetString(reader.GetOrdinal("celular_paseador")),
                            CorreoPaseador = reader.GetString(reader.GetOrdinal("correo_paseador")),
                            EstaDisponible = reader.GetBoolean(reader.GetOrdinal("esta_disponible"))
                        });
                    }
                    return Ok(paseadores);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                // Log the error details
                Console.WriteLine("Column not found: " + ex.Message);
                return StatusCode(500, "Internal server error. Column not found.");
            }
            catch (Exception ex)
            {
                // Log the error details
                Console.WriteLine("An error occurred: " + ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }


        // POST: api/paseador/solicitud
        [HttpPost("solicitud")]
        public async Task<IActionResult> EnviarSolicitudPaseo([FromBody] SolicitudPaseoDto solicitud)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("EnviarSolicitudPaseo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_canino", solicitud.IdCanino);
                command.Parameters.AddWithValue("@cedula_paseador", solicitud.CedulaPaseador);

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Solicitud enviada exitosamente" });
            }
        }


        // GET: api/paseador/{cedula}/solicitudes
        [HttpGet("{cedula}/solicitudes")]
        public async Task<IActionResult> GetSolicitudesPaseador(string cedula)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("ObtenerSolicitudesPaseador", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@cedula_paseador", cedula);

                var solicitudes = new List<SolicitudPendienteDto>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var solicitud = new SolicitudPendienteDto
                        {
                            // Cambiamos los nombres para que coincidan con la base de datos
                            IdSolicitud = reader.GetInt32(reader.GetOrdinal("id_solicitud")),
                            FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("fecha_solicitud")),
                            EstadoSolicitud = reader.GetString(reader.GetOrdinal("estado_solicitud")),
                            NombreCanino = reader.GetString(reader.GetOrdinal("nombre_canino")),
                            NombreDueno = reader.GetString(reader.GetOrdinal("nombre_dueno")),
                            CelularDueno = reader.GetString(reader.GetOrdinal("celular_dueno"))
                        };
                        solicitudes.Add(solicitud);
                    }
                }
                return Ok(solicitudes);
            }
        }

        // PUT: api/paseador/solicitud/{id}/responder
        [HttpPut("solicitud/{id}/responder")]
        public async Task<IActionResult> ResponderSolicitud(int id, [FromBody] RespuestaSolicitudDto respuesta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("ResponderSolicitudPaseo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_solicitud", id);
                command.Parameters.AddWithValue("@aceptada", respuesta.Aceptada);
                command.Parameters.AddWithValue("@cedula_paseador", respuesta.CedulaPaseador);

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Respuesta procesada exitosamente" });
            }
        }

        [HttpDelete("solicitud/{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("EliminarSolicitudPaseo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_solicitud", id);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return Ok(new { message = "Solicitud eliminada exitosamente." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = $"Error al eliminar la solicitud: {ex.Message}" });
                }
            }
        }

        // Get: obetner id paseo po id soli 
        [HttpGet("ObtenerIdPaseoPorIdSolicitud/{idSolicitud}")]
        public async Task<IActionResult> ObtenerIdPaseoPorIdSolicitud(int idSolicitud)
        {
            var paseo = await _context.Paseo
                .Where(p => p.IdSolicitud == idSolicitud)
                .Select(p => p.IdPaseo)
                .FirstOrDefaultAsync();

            if (paseo == 0) // Si no encuentra el paseo
            {
                return NotFound("No se encontró un paseo para esta solicitud.");
            }

            return Ok(paseo);
        }


        // PUT: api/paseador/paseo/{id}/finalizar
        [HttpPut("paseo/{id}/finalizar")]
        public async Task<IActionResult> FinalizarPaseo(int id, [FromBody] FinalizarPaseoDto request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("FinalizarPaseo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_paseo", id);
                command.Parameters.AddWithValue("@cedula_paseador", request.CedulaPaseador);

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Paseo finalizado exitosamente" });
            }
        }


        // PUT: api/paseador/paseo/{idPaseo}/{accion}
        [HttpPut("paseo/{idSolicitud}/{accion}")]
        public async Task<IActionResult> ActualizarEstadoPaseo(int idSolicitud, string accion, [FromBody] ActualizacionPaseoDto request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Validar la acción
                if (accion != "iniciar" && accion != "finalizar")
                {
                    return BadRequest("Acción no válida. Debe ser 'iniciar' o 'finalizar'");
                }

                var procedureName = accion == "iniciar" ? "IniciarPaseo" : "FinalizarPaseo";

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                    command.Parameters.AddWithValue("@CedulaPaseador", request.CedulaPaseador);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return Ok(new { message = $"Paseo {(accion == "iniciar" ? "iniciado" : "finalizado")} exitosamente" });
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new { message = "Error al actualizar el estado del paseo", error = ex.Message });
                    }
                }
            }

        }


        [HttpDelete("paseo/{id}")]
        public async Task<IActionResult> EliminarPaseo(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("EliminarPaseo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_paseo", id);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return Ok(new { message = "Paseo eliminado exitosamente." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = $"Error al eliminar el paseo: {ex.Message}" });
                }
            }
        }

        // DTOs
        public class SolicitudPaseoDto
        {
            public int IdCanino { get; set; }
            public string CedulaPaseador { get; set; }
        }

        public class RespuestaSolicitudDto
        {
            public bool Aceptada { get; set; }
            public string CedulaPaseador { get; set; }
        }

        public class FinalizarPaseoDto
        {
            public string CedulaPaseador { get; set; }
        }

        public class SolicitudPendienteDto
        {
            public int IdSolicitud { get; set; }
            public DateTime FechaSolicitud { get; set; }
            public string EstadoSolicitud { get; set; }
            public string NombreCanino { get; set; }
            public string NombreDueno { get; set; }
            public string CelularDueno { get; set; }
        }

        public class PaseadorDto
        {
            public string CedulaPaseador { get; set; }
            public string NombrePaseador { get; set; }
            public string CelularPaseador { get; set; }
            public string CorreoPaseador { get; set; }
            public bool EstaDisponible { get; set; }
        }

        public class ActualizacionPaseoDto
        {
            public string CedulaPaseador { get; set; }
        }
    }
}