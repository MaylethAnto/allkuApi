using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DuenoController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public DuenoController(AllkuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dueno>>> GetDuenos()
        {
            return await _context.Dueno.ToListAsync();
        }

        [HttpGet("{cedula}")]
        public async Task<ActionResult<Dueno>> GetDueno(string cedula)
        {
            var dueno = await _context.Dueno.FindAsync(cedula);

            if (dueno == null)
            {
                return NotFound();
            }

            return dueno;
        }

        [HttpPost]
        public async Task<ActionResult<Dueno>> PostDueno(Dueno dueno)
        {
            _context.Dueno.Add(dueno);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDueno), new { cedula = dueno.cedula_dueno }, dueno);
        }

        [HttpPut("{cedula}")]
        public async Task<IActionResult> PutDueno(string cedula, Dueno dueno)
        {
            if (cedula != dueno.cedula_dueno)
            {
                return BadRequest();
            }

            _context.Entry(dueno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DuenoExists(cedula))
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

        [HttpDelete("{cedula}")]
        public async Task<IActionResult> DeleteDueno(string cedula)
        {
            var dueno = await _context.Dueno.FindAsync(cedula);
            if (dueno == null)
            {
                return NotFound();
            }

            _context.Dueno.Remove(dueno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DuenoExists(string cedula)
        {
            return _context.Dueno.Any(e => e.cedula_dueno == cedula);
        }
    }
}