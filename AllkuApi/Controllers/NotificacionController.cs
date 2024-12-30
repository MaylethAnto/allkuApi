using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllkuApi.Data;
using AllkuApi.Models;
using AllkuApi.DataTransferObjects_DTO_;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
    private readonly AllkuDbContext _context;

    public NotificacionController(AllkuDbContext context)
    {
        _context = context;
    }

    [HttpPost("enviar")]
    public async Task<IActionResult> EnviarNotificacion([FromBody] NotificacionDto notificacionDto)
    {
        var canino = await _context.Canino
            .Include(c => c.Dueno)
            .FirstOrDefaultAsync(c => c.IdCanino == notificacionDto.IdCanino);

        if (canino == null)
        {
            return NotFound("Canino no encontrado.");
        }

        var notificacion = new Notificacion
        {
            Mensaje = notificacionDto.Mensaje,
            NumeroPaseador = notificacionDto.NumeroPaseador,
            CedulaDueno = canino.CedulaDueno,
            Fecha = DateTime.UtcNow,
            Leida = false
        };

        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("check")]
    public async Task<ActionResult<int>> CheckForNotifications(string cedulaDueno)
    {
        var count = await _context.Notificaciones
            .CountAsync(n => n.CedulaDueno == cedulaDueno && !n.Leida);
        return Ok(count);
    }

    [HttpGet("ultima")]
    public async Task<ActionResult<NotificacionDto>> GetLatestNotification(string cedulaDueno)
    {
        var notificacion = await _context.Notificaciones
            .Where(n => n.CedulaDueno == cedulaDueno && !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .FirstOrDefaultAsync();

        if (notificacion == null)
        {
            return NotFound();
        }

        var notificacionDto = new NotificacionDto
        {
            IdCanino = notificacion.IdNotificacion,
            Mensaje = notificacion.Mensaje,
            NumeroPaseador = notificacion.NumeroPaseador
        };

        return Ok(notificacionDto);
    }

    [HttpPut("marcarComoLeida")]
    public async Task<IActionResult> MarcarNotificacionComoLeida(int idNotificacion)
    {
        var notificacion = await _context.Notificaciones.FindAsync(idNotificacion);
        if (notificacion == null)
        {
            return NotFound();
        }

        notificacion.Leida = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}