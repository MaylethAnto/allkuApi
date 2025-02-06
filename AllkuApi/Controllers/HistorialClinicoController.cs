using AllkuApi.Controllers;
using AllkuApi.Data;
using AllkuApi.DataTransferObjects_DTO_;
using AllkuApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialClinicoController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public HistorialClinicoController(AllkuDbContext context)
        {
            _context = context;
        }

        // Acción GET para obtener los historiales clínicos, ahora con el DTO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> GetHistorialClinico()
        {
            var historial = await _context.Historiales_Clinico
                .Include(h => h.Canino)
                .Select(h => new HistorialClinicoDto
                {
                    id_historial = h.IdHistorial,
                    fecha_historial = h.FechaHistorial,
                    tipo_historial = h.TipoHistorial,
                    descripcion_historial = h.DescripcionHistorial,
                    id_canino = h.IdCanino
                })
                .ToListAsync();

            return Ok(historial);
        }

        // GET: api/Canino/HistorialesClinico/{idCanino}
        [HttpGet("Canino/{idCanino}")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> GetHistorialesPorCanino(int idCanino)
        {
            try
            {
                var historiales = await _context.Historiales_Clinico
                    .Include(h => h.Canino)
                    .Where(h => h.IdCanino == idCanino)
                    .OrderByDescending(h => h.FechaHistorial)
                    .Select(h => new HistorialClinicoDto
                    {
                        id_historial = h.IdHistorial,
                        fecha_historial = h.FechaHistorial,
                        tipo_historial = h.TipoHistorial,
                        descripcion_historial = h.DescripcionHistorial,
                        id_canino = h.IdCanino
                    })
                    .ToListAsync();

                if (!historiales.Any())
                {
                    return NotFound($"No se encontraron historiales clínicos para el canino con ID: {idCanino}");
                }

                return Ok(historiales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // Acción POST para crear un historial clínico
        [HttpPost]
        public async Task<ActionResult> CreateHistorial([FromBody] HistorialRequest historialRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el id del canino existe 
            var canino = await _context.Canino
                .Include(c => c.Dueno)
                .FirstOrDefaultAsync(c => c.IdCanino == historialRequest.id_canino);
            if (canino == null)
            {
                return NotFound($"Canino con id {historialRequest.id_canino} no encontrado.");
            }

            var historial = new Historial_Clinico
            {
                FechaHistorial = historialRequest.fecha_historial,
                TipoHistorial = historialRequest.tipo_historial,
                DescripcionHistorial = historialRequest.descripcion_historial,
                IdCanino = historialRequest.id_canino,
                Canino = canino
            };

            // Guardar el Historial 
            _context.Historiales_Clinico.Add(historial);
            await _context.SaveChangesAsync();

            // Crear la notificación para el dueño del canino
            var notificacion = new Notificacion
            {
                Mensaje = $"Recordatorio: {historial.DescripcionHistorial}",
                CedulaDueno = canino.Dueno.CedulaDueno,
                Fecha = historial.FechaHistorial, // Usar la fecha del historial para la notificación
                Leida = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok("Historial Registrado Exitosamente");
        }

        // Acción PUT para actualizar un historial clínico
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistorial(int id, [FromBody] Historial_Clinico historial)
        {
            if (id != historial.IdHistorial)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(historial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Historiales_Clinico.Any(e => e.IdHistorial == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Acción DELETE para eliminar un historial clínico
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorial(int id)
        {
            var historialClinico = await _context.Historiales_Clinico.FindAsync(id);
            if (historialClinico == null)
            {
                return NotFound();
            }

            _context.Historiales_Clinico.Remove(historialClinico);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return NoContent();
        }
    }
}