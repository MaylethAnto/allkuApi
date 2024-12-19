using AllkuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;

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
        [HttpGet]
        public async Task<IActionResult> GetHistorialClinico()
        {
            var historial = await _context.Historial_Clinico.ToListAsync();
            return Ok(historial);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHistorial([FromBody] Historial_Clinico historial)
        {
            if (ModelState.IsValid)
            {
                _context.Historial_Clinico.Add(historial);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetHistorialClinico), new { id = historial.id_historial }, historial);

            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistorial(int id, [FromBody] Historial_Clinico historial)
        {
            if (id != historial.id_historial) return BadRequest();

            _context.Entry(historial).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorial(int id, [FromBody] Historial_Clinico historial)
        {
            var historial_Clinico = await _context.Historial_Clinico.FindAsync(id);
            if (historial == null) return NotFound();

            _context.Historial_Clinico.Remove(historial);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
