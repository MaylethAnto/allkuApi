using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AllkuApi.Models;
using AllkuApi.Data;
using AllkuApi.Dtos;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AllkuDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AllkuDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private string GenerarToken(Manejo_Perfiles usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.nombre_usuario),
        new Claim(ClaimTypes.Role, usuario.tipo_perfil),
        new Claim("id_perfil", usuario.id_perfil.ToString())
    };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var usuario = await _context.Manejo_Perfiles
                .FirstOrDefaultAsync(u => u.nombre_usuario == loginDto.NombreUsuario &&
                                          u.contrasena == loginDto.Contrasena);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas");

            var token = GenerarToken(usuario);
            return Ok(new { token, tipoPerfil = usuario.tipo_perfil });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            // Validar que el usuario no exista
            if (await _context.Manejo_Perfiles.AnyAsync(x => x.nombre_usuario == registerDto.NombreUsuario))
                return BadRequest("Usuario ya existe");

            var nuevoUsuario = new Manejo_Perfiles
            {
                nombre_usuario = registerDto.NombreUsuario,
                contrasena = registerDto.Contrasena,
                tipo_perfil = registerDto.TipoPerfil,
                id_administrador = registerDto.IdAdministrador,
                cedula_dueno = registerDto.CedulaDueno,
                id_paseador = registerDto.IdPaseador
            };

            _context.Manejo_Perfiles.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente");
        }
    }
}