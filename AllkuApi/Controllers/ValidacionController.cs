using AllkuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ValidacionController : ControllerBase
{
    private readonly AllkuDbContext _context;
    private readonly ILogger<ValidacionController> _logger;

    public ValidacionController(AllkuDbContext context, ILogger<ValidacionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("verificar-cedula/{cedula}")]
    public async Task<IActionResult> VerificarCedula(string cedula)
    {
        try
        {
            var existe = await _context.Administrador.AnyAsync(a => a.CedulaAdministrador == cedula) ||
                        await _context.Dueno.AnyAsync(d => d.CedulaDueno == cedula) ||
                        await _context.Paseador.AnyAsync(p => p.CedulaPaseador == cedula);

            return Ok(new { existe });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al verificar cédula: {cedula}");
            return StatusCode(500, new { mensaje = "Error al verificar la cédula" });
        }
    }

    [HttpGet("verificar-usuario/{usuario}")]
    public async Task<IActionResult> VerificarUsuario(string usuario)
    {
        try
        {
            var existe = await _context.ManejoPerfiles.AnyAsync(m => m.NombreUsuario == usuario);
            return Ok(new { existe });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al verificar usuario: {usuario}");
            return StatusCode(500, new { mensaje = "Error al verificar el usuario" });
        }
    }

    [HttpGet("verificar-correo/{correo}")]
    public async Task<IActionResult> VerificarCorreo(string correo)
    {
        try
        {
            var existe = await _context.Administrador.AnyAsync(a => a.CorreoAdministrador == correo) ||
                        await _context.Dueno.AnyAsync(d => d.CorreoDueno == correo) ||
                        await _context.Paseador.AnyAsync(p => p.CorreoPaseador == correo);

            return Ok(new { existe });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al verificar correo: {correo}");
            return StatusCode(500, new { mensaje = "Error al verificar el correo" });
        }
    }
}