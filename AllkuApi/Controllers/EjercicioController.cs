using AllkuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;


namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EjercicioController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public EjercicioController(AllkuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEjercicios()
        {
            var ejercicios = await _context.Ejercicios.ToListAsync();
            return Ok(ejercicios);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEjercicio([FromBody] Ejercicio ejercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Ejercicios.Add(ejercicio);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEjercicios), new { id = ejercicio.id_ejercicio }, ejercicio);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEjercicio(int id, [FromBody] Ejercicio ejercicio)
        {
            if (id != ejercicio.id_ejercicio) return BadRequest();

            _context.Entry(ejercicio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEjercicio(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio == null) return NotFound();

            _context.Ejercicios.Remove(ejercicio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
