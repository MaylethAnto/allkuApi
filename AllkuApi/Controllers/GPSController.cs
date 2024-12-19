using AllkuApi.Data;
using AllkuApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPSController : ControllerBase
    {
        private readonly AllkuDbContext _context;

        public GPSController(AllkuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGPS()
        {
            var gps = await _context.GPS.ToListAsync();
            return Ok(gps);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGPS([FromBody] GPS gPS)
        {
            if (ModelState.IsValid)
            {
                _context.GPS.Add(gPS);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetGPS), new { id = gPS.id_gps }, gPS);

            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGPS(int id, [FromBody] GPS gPS)
        {
            if (id != gPS.id_gps) return BadRequest();

            _context.Entry(gPS).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGPS(int id, [FromBody] GPS gPS)
        {
            var gPS1 = await _context.GPS.FindAsync(id);
            if (gPS == null) return NotFound();

            _context.GPS.Remove(gPS);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
