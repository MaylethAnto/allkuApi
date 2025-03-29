using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
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

        [HttpGet("distancia/{id_canino}")]
        public async Task<ActionResult<DistanciaRecorrida>> GetDistanciaRecorrida(int id_canino)
        {
            if (id_canino <= 0)
            {
                return BadRequest("El ID del canino es inválido.");
            }

            var gpsData = await _context.GPS
                                        .Where(g => g.IdCanino == id_canino)
                                        .OrderBy(g => g.FechaGps)
                                        .ToListAsync();

            if (!gpsData.Any())
            {
                return NotFound();
            }

            double totalDistance = 0.0;
            for (int i = 1; i < gpsData.Count; i++)
            {
                totalDistance += CalcularDistancia(
                    (double)gpsData[i - 1].InicioLatitud,
                    (double)gpsData[i - 1].InicioLongitud,
                    (double)gpsData[i].FinLatitud,
                    (double)gpsData[i].FinLongitud);
            }

            var distanciaRecorrida = new DistanciaRecorrida
            {
                DistanciaTotal = (decimal)totalDistance
            };

            return Ok(distanciaRecorrida);
        }

        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radio de la Tierra en kilómetros
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var lat1Rad = ToRadians(lat1);
            var lat2Rad = ToRadians(lat2);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distancia en kilómetros
        }

        private double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
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
                            DistanciaKm = x.Paseo.DistanciaKm ?? 0
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