using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;
using System.Threading.Tasks;

namespace AllkuApi.Data
{
    public class AllkuDbContext : DbContext
    {
        public AllkuDbContext(DbContextOptions<AllkuDbContext> options) : base(options) { }

        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<Canino> Canino { get; set; }
        public DbSet<Dueno> Dueno { get; set; }
        public DbSet<GPS> GPS { get; set; }
        public DbSet<Historial_Clinico> Historiales_Clinico { get; set; }
        public DbSet<Manejo_Perfiles> ManejoPerfiles { get; set; }
        public DbSet<Paseador> Paseador { get; set; }
        public DbSet<Receta> Receta { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Paseo> Paseo { get; set; }
        public DbSet<SolicitudPaseo> SolicitudPaseo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().ToTable("Administrador");
            modelBuilder.Entity<Manejo_Perfiles>().ToTable("Manejo_Perfiles");
            modelBuilder.Entity<Canino>().ToTable("Canino");

            modelBuilder.Entity<Canino>()
                     .HasOne(c => c.Dueno)
                     .WithMany(d => d.Caninos)
                     .HasForeignKey(c => c.CedulaDueno);

            modelBuilder.Entity<Historial_Clinico>().ToTable("Historial_Clinico");

            base.OnModelCreating(modelBuilder);

            // Configurar Notificaciones
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Dueno)
                .WithMany(d => d.Notificaciones)
                .HasForeignKey(n => n.CedulaDueno);

            modelBuilder.Entity<Manejo_Perfiles>(entity =>
            {
                // Configuración de Administrador
                entity.HasOne(mp => mp.Administrador)
                    .WithMany(a => a.Manejo_Perfiles)
                    .HasForeignKey(mp => mp.CedulaAdministrador)
                    .IsRequired(false)  // Hacer la relación opcional
                    .OnDelete(DeleteBehavior.Restrict);

                // Configuración de Dueño
                entity.HasOne(mp => mp.Dueno)
                    .WithMany(d => d.Manejo_Perfiles)
                    .HasForeignKey(mp => mp.CedulaDueno)
                    .IsRequired(false)  // Hacer la relación opcional
                    .OnDelete(DeleteBehavior.Restrict);

                // Configuración de Paseador
                entity.HasOne(mp => mp.Paseador)
                    .WithMany(p => p.Manejo_Perfiles)
                    .HasForeignKey(mp => mp.CedulaPaseador)
                    .IsRequired(false)  // Hacer la relación opcional
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de GPS

            modelBuilder.Entity<GPS>()
             .HasOne(g => g.Canino)
             .WithMany(c => c.GPS) // Assuming Canino has a collection of GPS
             .HasForeignKey(g => g.IdCanino);

            // Configuración de Paseo
            modelBuilder.Entity<Paseo>(entity =>
            {
                entity.ToTable("Paseo");
                entity.HasKey(e => e.IdPaseo);

                // Configuración de propiedades
                entity.Property(e => e.FechaInicio)
                    .IsRequired();

                entity.Property(e => e.FechaFin)
                    .IsRequired();

                entity.Property(e => e.DistanciaKm)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)"); // ← ¡Clave para evitar errores!

                entity.Property(e => e.EstadoPaseo)
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // Configuración de SolicitudPaseo
            modelBuilder.Entity<SolicitudPaseo>(entity =>
            {
                entity.ToTable("Solicitud_Paseo");
                entity.HasKey(e => e.IdSolicitud);
                entity.Property(e => e.EstadoSolicitud).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("GETDATE()");
            });

            base.OnModelCreating(modelBuilder);
        }

        // Método para iniciar sesión (buscar en Manejo_Perfiles por nombre de usuario y contraseña)
        public async Task<Manejo_Perfiles> IniciarSesion(string nombreUsuario, string contrasena)
        {
            return await ManejoPerfiles
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);
        }
    }
}