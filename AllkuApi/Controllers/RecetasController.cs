using AllkuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;


namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public RecetasController(AllkuDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetRecetas()
        {
            var recetas = await _context.Receta.ToListAsync();
            return Ok(recetas);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRecetas([FromBody] Receta receta)
        {
            if (ModelState.IsValid)
            {
                _context.Receta.Add(receta);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRecetas), new { id = receta.id_receta }, receta);

            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceta(int id, [FromBody] Receta receta) 
        {
            if (id != receta.id_receta) return BadRequest();

            _context.Entry(receta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceta(int id, [FromBody] Receta receta) 
        {
            var receta1 = await _context.Receta.FindAsync(id);
            if (receta == null) return NotFound();

            _context.Receta.Remove(receta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
    }
}
