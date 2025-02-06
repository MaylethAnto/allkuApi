using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using AllkuApi.Models;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.DataTransferObjects_DTO_;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AllkuDbContext _context;

        public GpsController(IConfiguration configuration, AllkuDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<GPSDto>> PostGPS(GPSDto gpsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gps = new GPS
            {
                IdCanino = gpsDto.IdCanino,
                FechaGps = gpsDto.FechaGps,
                DistanciaKm = gpsDto.DistanciaKm,
                InicioLatitud = gpsDto.InicioLatitud.HasValue ? Math.Round(gpsDto.InicioLatitud.Value, 7) : (decimal?)null,
                InicioLongitud = gpsDto.InicioLongitud.HasValue ? Math.Round(gpsDto.InicioLongitud.Value, 7) : (decimal?)null,
                FinLatitud = gpsDto.FinLatitud.HasValue ? Math.Round(gpsDto.FinLatitud.Value, 7) : (decimal?)null,
                FinLongitud = gpsDto.FinLongitud.HasValue ? Math.Round(gpsDto.FinLongitud.Value, 7) : (decimal?)null
            };

            _context.GPS.Add(gps);
            await _context.SaveChangesAsync();

            gpsDto.IdGps = gps.IdGps; // Asignar el ID generado automáticamente

            return CreatedAtAction(nameof(GetGPS), new { id = gps.IdGps }, gpsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GPSDto>> GetGPS(int id)
        {
            // Buscar la entrada de GPS por ID
            var gps = await _context.GPS.FindAsync(id);
            if (gps == null)
            {
                return NotFound();
            }

            // Crear el DTO con las conversiones necesarias para los campos nullables
            var gpsDto = new GPSDto
            {
                IdGps = gps.IdGps,
                IdCanino = gps.IdCanino, // Asumiendo que IdCanino no es nullable
                FechaGps = gps.FechaGps ?? DateTime.MinValue, // Convertir DateTime? a DateTime
                DistanciaKm = gps.DistanciaKm ?? 0m, // Convertir decimal? a decimal
                InicioLatitud = gps.InicioLatitud ?? 0m, // Convertir decimal? a decimal
                InicioLongitud = gps.InicioLongitud ?? 0m, // Convertir decimal? a decimal
                FinLatitud = gps.FinLatitud ?? 0m, // Convertir decimal? a decimal
                FinLongitud = gps.FinLongitud ?? 0m // Convertir decimal? a decimal
            };

            // Devolver el DTO como respuesta
            return Ok(gpsDto);
        }

        [HttpGet("distancia")]
        public async Task<ActionResult<DistanciaRecorrida>> GetDistanciaRecorrida(int id_canino)
        {
            if (id_canino <= 0)
            {
                return BadRequest("El ID del canino es inválido.");
            }

            var distancia = new DistanciaRecorrida();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("ObtenerDistanciaRecorrida", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_canino", id_canino);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows && await reader.ReadAsync())
                            {
                                distancia.DistanciaTotal = reader["DistanciaTotal"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["DistanciaTotal"])
                                    : 0;
                            }
                            else
                            {
                                distancia.DistanciaTotal = 0; // Si no hay filas, devolver 0
                            }
                        }
                    }
                }

                return Ok(distancia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("paseos-finalizados/{id_canino}")]
        public async Task<IActionResult> ObtenerPaseosFinalizados(int id_canino)
        {
            try
            {
                if (!await _context.Paseo.AnyAsync() || !await _context.SolicitudPaseo.AnyAsync())
                {
                    return NotFound("No hay paseos o solicitudes disponibles.");
                }

                var paseos = await _context.Paseo
                        .Join(
                            _context.SolicitudPaseo,
                            paseo => paseo.IdSolicitud,
                            solicitud => solicitud.IdSolicitud,
                            (paseo, solicitud) => new
                            {
                                Paseo = paseo,
                                Solicitud = solicitud
                            }
                        )
                        .Where(x => x.Solicitud.IdCanino == id_canino && x.Paseo.EstadoPaseo == "Finalizado")
                        .Select(x => new
                        {
                            FechaInicio = x.Paseo.FechaInicio != null ? x.Paseo.FechaInicio : DateTime.MinValue,
                            FechaFin = x.Paseo.FechaFin != null ? x.Paseo.FechaFin : DateTime.MinValue, // Asegúrate de que FechaFin no sea null
                            DistanciaKm = x.Paseo.DistanciaKm != null ? x.Paseo.DistanciaKm : 0
                        })
                        .ToListAsync();

                if (paseos == null || !paseos.Any())
                {
                    return NotFound("No se encontraron paseos finalizados para el canino especificado.");
                }

                return Ok(paseos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }




}