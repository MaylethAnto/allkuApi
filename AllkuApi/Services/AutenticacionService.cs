using AllkuApi.Data;
using AllkuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AllkuApi.Services
{
    public class AutenticacionService
    {
        private readonly AllkuDbContext _context;
        private readonly HashService _hashService;

        public AutenticacionService(AllkuDbContext context, HashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        public async Task<Manejo_Perfiles> IniciarSesion(string nombreUsuario, string contrasena)
        {
            var usuario = await _context.Manejo_Perfiles
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            if (!usuario.EstadoCuenta)
                throw new UnauthorizedAccessException("Cuenta bloqueada");

            if (!_hashService.VerifyPassword(contrasena, usuario.Contrasena))
            {
                usuario.IntentosFallidos++;

                if (usuario.IntentosFallidos >= 5)
                {
                    usuario.EstadoCuenta = false;
                }

                await _context.SaveChangesAsync();
                throw new UnauthorizedAccessException("Contraseña incorrecta");
            }

            // Reiniciar intentos fallidos
            usuario.IntentosFallidos = 0;
            usuario.UltimoInicioSesion = DateTime.Now;
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Manejo_Perfiles> RegistrarUsuario(string nombreUsuario, string contrasena, string rol)
        {
            // Verificar si el usuario ya existe
            if (await _context.Manejo_Perfiles.AnyAsync(u => u.NombreUsuario == nombreUsuario))
                throw new InvalidOperationException("El usuario ya existe");

            var nuevoUsuario = new Manejo_Perfiles
            {
                NombreUsuario = nombreUsuario,
                Contrasena = _hashService.HashPassword(contrasena),
                RolUsuario = rol,
                EstadoCuenta = true,
                IntentosFallidos = 0
            };

            _context.Manejo_Perfiles.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return nuevoUsuario;
        }
    }
}
