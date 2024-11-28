using AllkuApi.Data;
using AllkuApi.DataTransferObjects_DTO_;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaseadorController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public PaseadorController(AllkuDbContext context)
        {
            _context = context;
        }

        [HttpGet("CaninosConDuenos")]
        public async Task<ActionResult<IEnumerable<CaninoConDuenoDto>>> GetCaninosConDuenos()
        {
            var caninos = await _context.Canino
                .Include(c => c.CedulaDueno)  // Incluimos la relación con Dueno
                .Select(c => new CaninoConDuenoDto
                {
                    IdCanino = c.IdCanino,
                    NombreCanino = c.NombreCanino,
                    RazaCanino = c.RazaCanino,
                    EdadCanino = c.EdadCanino,
                    PesoCanino = c.PesoCanino,
                    NombreDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.NombreDueno).FirstOrDefault(),
                    DireccionDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.DireccionDueno).FirstOrDefault(),
                    CelularDueno = _context.Dueno.Where(d => d.CedulaDueno == c.CedulaDueno).Select(d => d.CelularDueno).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(caninos);
        }
    }

}

   

