using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllkuApi.Data;
using AllkuApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public AdministradorController(AllkuDbContext context)
        {
            _context = context;
        }

        // Obtener todos los paseadores
        [HttpGet("paseadores")]
        public async Task<IActionResult> GetPaseadores()
        {
            var paseadores = await _context.Paseador.ToListAsync();
            if (paseadores == null || paseadores.Count == 0)
            {
                return NotFound("No se encontraron paseadores.");
            }
            return Ok(paseadores);
        }

        // Obtener todos los dueños
        [HttpGet("dueños")]
        public async Task<IActionResult> GetDueños()
        {
            var duenos = await _context.Dueno.ToListAsync();
            if (duenos == null || duenos.Count == 0)
            {
                return NotFound("No se encontraron dueños.");
            }
            return Ok(duenos);
        }

        // Obtener todas las mascotas
        [HttpGet("caninos")]
        public async Task<IActionResult> GetCaninos()
        {
            var caninos = await _context.Canino.ToListAsync();
            if (caninos == null || caninos.Count == 0)
            {
                return NotFound("No se encontraron caninos.");
            }
            return Ok(caninos);
        }
    }
}
