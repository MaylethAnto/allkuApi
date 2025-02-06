using AllkuApi.Controllers;
using AllkuApi.Data;
using AllkuApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

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

        // GET: api/Recetas                    
        [HttpGet]
        public async Task<IActionResult> GetRecetas()
        {
            var recetas = await _context.Receta.ToListAsync();
            return Ok(recetas);
        }

        // POST: api/Recetas
        [HttpPost]
        public async Task<IActionResult> CreateReceta([FromForm] RecetaRequest createRecetaRequest)
        {
            if (ModelState.IsValid)
            {
                byte[]? fotoRecetaBytes = null;

                if (createRecetaRequest.foto_receta != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await createRecetaRequest.foto_receta.CopyToAsync(ms);
                        fotoRecetaBytes = ms.ToArray();
                    }
                }

                var receta = new Receta
                {
                    nombre_receta = createRecetaRequest.nombre_receta,
                    descripcion_receta = createRecetaRequest.descripcion_receta,
                    foto_receta = fotoRecetaBytes,
                    id_canino = createRecetaRequest.id_canino
                };

                _context.Receta.Add(receta);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRecetas), new { id = receta.id_receta }, new { message = "Receta registrada correctamente", receta });
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Recetas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceta(int id, [FromForm] RecetaRequest recetaRequest)
        {
            if (id != recetaRequest.id_receta) return BadRequest();

            var receta = await _context.Receta.FindAsync(id);
            if (receta == null) return NotFound();

            if (ModelState.IsValid)
            {
                byte[]? fotoRecetaBytes = null;

                if (recetaRequest.foto_receta != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await recetaRequest.foto_receta.CopyToAsync(ms);
                        fotoRecetaBytes = ms.ToArray();
                    }
                }

                receta.nombre_receta = recetaRequest.nombre_receta;
                receta.descripcion_receta = recetaRequest.descripcion_receta;
                receta.foto_receta = fotoRecetaBytes;
                receta.id_canino = recetaRequest.id_canino;

                _context.Entry(receta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Recetas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _context.Receta.FindAsync(id);
            if (receta == null) return NotFound();

            _context.Receta.Remove(receta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}