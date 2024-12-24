using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                .FirstOrDefaultAsync(c => c.IdCanino == id);

            if (canino == null)
            {
                return NotFound();
            }

            return canino;
        }

        [HttpPost("RegistrarCanino")]
        public async Task<ActionResult> PostCanino(CaninoRequest caninoRequest)
        {
            if (caninoRequest == null)
            {
                return BadRequest("Los datos del canino son requeridos.");
            }

            // Verificar si el dueño con la cédula existe
            var dueno = await _context.Dueno
                                      .FirstOrDefaultAsync(d => d.CedulaDueno == caninoRequest.CedulaDueno);

            if (dueno == null)
            {
                return BadRequest("El dueño con la cédula proporcionada no existe.");
            }

            // Crear una nueva instancia de Canino
            var canino = new Canino
            {
                NombreCanino = caninoRequest.NombreCanino,
                EdadCanino = caninoRequest.EdadCanino,
                RazaCanino = caninoRequest.RazaCanino,
                PesoCanino = caninoRequest.PesoCanino,
                FotoCanino = caninoRequest.FotoCanino,
                CedulaDueno = caninoRequest.CedulaDueno,
                Dueno = dueno
            };

            // Guardar el canino
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
    }
}