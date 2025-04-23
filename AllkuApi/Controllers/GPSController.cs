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
                // Verificar cada paso del proceso de forma independiente
                // 1. Verificar si el contexto existe
                if (_context == null)
                {
                    return StatusCode(500, "Error: Contexto de base de datos no disponible");
                }

                // 2. Verificar si las tablas existen
                if (_context.Paseo == null)
                {
                    return StatusCode(500, "Error: La tabla Paseo no está disponible en el contexto");
                }

                if (_context.SolicitudPaseo == null)
                {
                    return StatusCode(500, "Error: La tabla SolicitudPaseo no está disponible en el contexto");
                }

                // 3. Verificar si hay registros en las tablas (de forma segura)
                bool hayPaseos = false;
                bool haySolicitudes = false;

                try
                {
                    hayPaseos = await _context.Paseo.AnyAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al verificar tabla Paseo: {ex.Message}");
                }

                try
                {
                    haySolicitudes = await _context.SolicitudPaseo.AnyAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al verificar tabla SolicitudPaseo: {ex.Message}");
                }

                if (!hayPaseos || !haySolicitudes)
                {
                    return Ok(new { Message = "No hay paseos o solicitudes disponibles." });
                }

                // 4. Obtener las solicitudes del canino de forma segura
                List<int> solicitudesIds = new List<int>();
                try
                {
                    solicitudesIds = await _context.SolicitudPaseo
                        .Where(s => s.IdCanino == id_canino)
                        .Select(s => s.IdSolicitud)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al buscar solicitudes: {ex.Message}");
                }

                if (solicitudesIds.Count == 0)
                {
                    return Ok(new { Message = $"No hay solicitudes para el canino con ID {id_canino}." });
                }

                // 5. Obtener los paseos finalizados de forma manual
                List<object> resultados = new List<object>();

                // Usar ToList() para materializar los resultados primero
                List<Paseo> paseosRaw = new List<Paseo>();
                try
                {
                    paseosRaw = await _context.Paseo
                        .Where(p => solicitudesIds.Contains(p.IdSolicitud) && p.EstadoPaseo == "Finalizado")
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al obtener paseos: {ex.Message}");
                }

                // 6. Procesar cada paseo de forma individual
                foreach (var p in paseosRaw)
                {
                    try
                    {
                        resultados.Add(new
                        {
                            FechaInicio = p.FechaInicio != null ? p.FechaInicio.Value : DateTime.MinValue,
                            FechaFin = p.FechaFin != null ? p.FechaFin.Value : DateTime.MinValue,
                            DistanciaKm = p.DistanciaKm != null ? (double)p.DistanciaKm.Value : 0.0
                        });
                    }
                    catch (Exception ex)
                    {
                        // Registrar el error pero continuar con los demás paseos
                        // Esto nos ayuda a identificar qué paseo específico está causando problemas
                        Console.WriteLine($"Error al procesar paseo ID {p.IdPaseo}: {ex.Message}");
                    }
                }

                if (resultados.Count == 0)
                {
                    return Ok(new { Message = $"No se encontraron paseos finalizados para el canino con ID {id_canino}." });
                }

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener paseos finalizados: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGPS(int id, GPSDto gpsDto)
        {
            if (id != gpsDto.IdGps)
            {
                return BadRequest("ID in URL doesn't match ID in body");
            }

            var gps = await _context.GPS.FindAsync(id);
            if (gps == null)
            {
                return NotFound();
            }
         
            if (gpsDto.FinLatitud.HasValue)
                gps.FinLatitud = Math.Round(gpsDto.FinLatitud.Value, 7);

            if (gpsDto.FinLongitud.HasValue)
                gps.FinLongitud = Math.Round(gpsDto.FinLongitud.Value, 7);


            try
            {
                _context.Entry(gps).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GpsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        // PUT: api/Gps/finalizar/{id}
        [HttpPut("finalizar/{id}")]
        public async Task<IActionResult> FinalizarGps(int id, [FromBody] FinalizarGpsDto dto)
        {
            try
            {
                // Buscar el GPS existente
                var gps = await _context.GPS.FindAsync(id);
                if (gps == null)
                {
                    return NotFound($"No se encontró el registro GPS con ID {id}");
                }

                // Actualizar solo las coordenadas finales
                gps.FinLatitud = Math.Round(dto.FinLatitud, 7);
                gps.FinLongitud = Math.Round(dto.FinLongitud, 7);

                // Guardar cambios
                _context.Entry(gps).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Coordenadas finales actualizadas correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // DTO para finalizar GPS
        public class FinalizarGpsDto
        {
            public decimal FinLatitud { get; set; }
            public decimal FinLongitud { get; set; }
        }

        private bool GpsExists(int id)
        {
            return _context.GPS.Any(e => e.IdGps == id);
        }
    }
}