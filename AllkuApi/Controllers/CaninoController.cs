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
            return await _context.Canino.Include(c => c.CedulaDueno).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Canino>> GetCanino(int id)
        {
            var canino = await _context.Canino
            .FirstOrDefaultAsync(c => c.IdCanino == id);

            if (canino != null)
            {
                var dueno = await _context.Dueno
                    .FirstOrDefaultAsync(d => d.CedulaDueno == canino.CedulaDueno);
            }

            return canino;
        }

        [HttpPost("RegistrarCanino")]
        public async Task<ActionResult<Canino>> PostCanino(Canino canino)
        {
            if (canino == null)
            {
                return BadRequest("Los datos del canino son requeridos.");
            }

            // Verificar si el dueño con la cédula existe
            var dueno = await _context.Dueno
                                      .FirstOrDefaultAsync(d => d.CedulaDueno == canino.CedulaDueno);

            if (dueno == null)
            {
                return BadRequest("El dueño con la cédula proporcionada no existe.");
            }

            // Ahora solo guardamos el canino con la cédula del dueño
            _context.Canino.Add(canino);
            await _context.SaveChangesAsync();

            return Ok("Canino registrado exitosamente.");
        }

        

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCanino(int id, Canino canino)
        {
            if (id != canino.IdCanino)
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
            return _context.Canino.Any(e => e.IdCanino == id);
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarioPorCedula([FromQuery] string cedulaDueno)
        {
            var dueno = await _context.Dueno
          .Where(d => d.CedulaDueno == cedulaDueno)
          .FirstOrDefaultAsync();

            if (dueno == null)
            {
                return NotFound("No se encontró al dueño con esa cédula");
            }

            return Ok(dueno);
        }

    }
}