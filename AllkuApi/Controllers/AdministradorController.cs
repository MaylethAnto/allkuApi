using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public AdministradorController(AllkuDbContext context)
        {
            _context = context;
        }

        // Obtener todos los paseadores
        [HttpGet("paseadores")]
        public async Task<IActionResult> GetPaseadores()
        {
            var paseadores = await _context.Paseador
                .Select(p => new
                {
                    p.CedulaPaseador,
                    p.NombrePaseador,
                    IdCanino = p.IdCanino // Permitirá nulos
                })
                .ToListAsync();

            return Ok(paseadores);
        }

        [HttpDelete("paseadores/{cedula}")]
        public async Task<IActionResult> DeletePaseador(string cedula)
        {
            var paseador = await _context.Paseador
                .FirstOrDefaultAsync(p => p.CedulaPaseador == cedula);

            if (paseador == null)
            {
                return NotFound(new { message = "Paseador no encontrado." });
            }

            _context.Paseador.Remove(paseador);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Paseador eliminado exitosamente.",
                paseador = new
                {
                    paseador.CedulaPaseador,
                    paseador.NombrePaseador,
                    IdCanino = paseador.IdCanino
                }
            });
        }

        //Endpoint para activar y desactivar el paseador

        [HttpPut("paseadores/{cedula}/estado")]
        public async Task<IActionResult> TogglePaseadorEstado(string cedula)
        {
            var paseador = await _context.Paseador
                .FirstOrDefaultAsync(p => p.CedulaPaseador == cedula);

            if (paseador == null)
            {
                return NotFound(new { message = "Paseador no encontrado." });
            }

            // Toggle the availability status
            paseador.EstaDisponible = !paseador.EstaDisponible;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = paseador.EstaDisponible
                    ? "Paseador activado exitosamente."
                    : "Paseador desactivado exitosamente.",
                paseador = new
                {
                    paseador.CedulaPaseador,
                    paseador.NombrePaseador,
                    paseador.EstaDisponible
                }
            });
        }


        // Obtener todos los dueños
        [HttpGet("dueños")]
        public async Task<IActionResult> GetDueños()
        {
            var duenos = await _context.Dueno.ToListAsync();
            if (duenos == null || duenos.Count == 0)
            {
                return NotFound("No se encontraron dueños.");
            }
            return Ok(duenos);
        }

        [HttpDelete("duenos/{cedula}")]
        public async Task<IActionResult> DeleteDueno(string cedula)
        {
            var dueno = await _context.Dueno
                .FirstOrDefaultAsync(d => d.CedulaDueno == cedula);

            if (dueno == null)
            {
                return NotFound(new { message = "Dueño no encontrado." });
            }

            _context.Dueno.Remove(dueno);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dueño eliminado exitosamente.",
                dueno = new
                {
                    dueno.CedulaDueno,
                    dueno.NombreDueno,
                    dueno.DireccionDueno,
                    dueno.CelularDueno,
                    dueno.CorreoDueno
                }
            });
        }

        // Obtener todas las mascotas
        [HttpGet("caninos")]
        public async Task<IActionResult> GetCaninos()
        {
            var caninos = await _context.Canino.ToListAsync();
            if (caninos == null || caninos.Count == 0)
            {
                return NotFound("No se encontraron caninos.");
            }
            return Ok(caninos);
        }
    }


}