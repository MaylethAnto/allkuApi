using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaninoController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public CaninoController(AllkuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Canino>>> GetCaninos()
        {
            return await _context.Canino.Include(c => c.Dueno).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Canino>> GetCanino(int id)
        {
            var canino = await _context.Canino
                .Include(c => c.Dueno)
                .FirstOrDefaultAsync(c => c.id_canino == id);

            if (canino == null)
            {
                return NotFound();
            }

            return canino;
        }

        [HttpPost]
        public async Task<ActionResult<Canino>> PostCanino(Canino canino)
        {
            _context.Canino.Add(canino);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCanino), new { id = canino.id_canino }, canino);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCanino(int id, Canino canino)
        {
            if (id != canino.id_canino)
            {
                return BadRequest();
            }

            _context.Entry(canino).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaninoExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCanino(int id)
        {
            var canino = await _context.Canino.FindAsync(id);
            if (canino == null)
            {
                return NotFound();
            }

            _context.Canino.Remove(canino);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CaninoExists(int id)
        {
            return _context.Canino.Any(e => e.id_canino == id);
        }
    }
}