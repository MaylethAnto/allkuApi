using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using AllkuApi.DataTransferObjects_DTO_;

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
        public async Task<ActionResult<IEnumerable<CaninoDto>>> GetCaninos()
        {
            var caninos = await _context.Canino
                .Include(c => c.Dueno)
                .Select(c => new CaninoDto
                {
                    IdCanino = c.IdCanino,
                    NombreCanino = c.NombreCanino,
                    EdadCanino = c.EdadCanino,
                    RazaCanino = c.RazaCanino,
                    PesoCanino = c.PesoCanino,
                    FotoCanino = c.FotoCanino,
                    NombreDueno = c.Dueno.NombreDueno
                })
                .ToListAsync();

            return Ok(caninos);
        }

        [HttpGet("caninosPorCedula")]
        public async Task<ActionResult<IEnumerable<CaninoDto>>> GetCaninosByCedulaDueno(string cedulaDueno)
        {
            var dueno = await _context.Dueno
                .Include(d => d.Caninos)
                .FirstOrDefaultAsync(d => d.CedulaDueno == cedulaDueno);

            if (dueno == null)
            {
                return NotFound("Dueño no encontrado.");
            }

            var caninosDto = dueno.Caninos.Select(c => new CaninoDto
            {
                IdCanino = c.IdCanino,
                NombreCanino = c.NombreCanino,
                EdadCanino = c.EdadCanino,
                RazaCanino = c.RazaCanino,
                PesoCanino = c.PesoCanino,
                FotoCanino = c.FotoCanino
            }).ToList();

            return Ok(caninosDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaninoDto>> GetCanino(int id)
        {
            var canino = await _context.Canino
                .Include(c => c.Dueno)
                .Where(c => c.IdCanino == id)
                .Select(c => new CaninoDto
                {
                    IdCanino = c.IdCanino,
                    NombreCanino = c.NombreCanino,
                    EdadCanino = c.EdadCanino,
                    RazaCanino = c.RazaCanino,
                    PesoCanino = c.PesoCanino,
                    FotoCanino = c.FotoCanino,
                    NombreDueno = c.Dueno.NombreDueno
                })
                .FirstOrDefaultAsync();

            if (canino == null)
            {
                return NotFound();
            }

            return Ok(canino);
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