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

        // GET: api/Administrador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrador>>> GetAdministradores()
        {
            return await _context.Administrador.ToListAsync();
        }

        // GET: api/Administrador/{cedula}
        [HttpGet("{id}")]
        public async Task<ActionResult<Administrador>> GetAdministrador(string id)
        {
            var administrador = await _context.Administrador.FindAsync(id);

            if (administrador == null)
            {
                return NotFound();
            }

            return administrador;
        }


        // POST: api/Administrador
        [HttpPost]
        public async Task<IActionResult> PostAdministrador([FromBody] Administrador administrador)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Administrador.Add(administrador);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAdministrador), new { id = administrador.CedulaAdministrador }, administrador);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Hubo un error al guardar el administrador.", error = ex.Message });
            }
        }





        // PUT: api/Administrador/{cedula}
        [HttpPut("{cedula}")]
        public async Task<IActionResult> PutAdministrador(string cedula, Administrador administrador)
        {
            if (cedula != administrador.CedulaAdministrador)
            {
                return BadRequest();
            }

            _context.Entry(administrador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministradorExists(cedula))
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

        // DELETE: api/Administrador/{cedula}
        [HttpDelete("{cedula}")]
        public async Task<IActionResult> DeleteAdministrador(string cedula)
        {
            var administrador = await _context.Administrador.FindAsync(cedula);
            if (administrador == null)
            {
                return NotFound();
            }

            _context.Administrador.Remove(administrador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdministradorExists(string cedula)
        {
            return _context.Administrador.Any(e => e.CedulaAdministrador == cedula);
        }
    }
}