using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;

namespace AllkuApi.Data
{
    public class AllkuDbContext : DbContext
    {
        public AllkuDbContext(DbContextOptions<AllkuDbContext> options) : base(options) { }

        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<Canino> Canino { get; set; }
        public DbSet<Dueno> Dueno { get; set; }
        public DbSet<Ejercicio> Ejercicio { get; set; }
        public DbSet<GPS> GPS { get; set; }
        public DbSet<Historial_Clinico> Historial_Clinico { get; set; }
        public DbSet<Manejo_Perfiles> Manejo_Perfiles { get; set; }
        public DbSet<Paseador> Paseador { get; set; }
        public DbSet<Receta> Receta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>()
                .HasIndex(a => a.correo_administrador)
                .IsUnique();

            modelBuilder.Entity<Administrador>()
                .HasIndex(a => a.usuario_administrador)
                .IsUnique();

            modelBuilder.Entity<Dueno>()
                .HasIndex(d => d.correo_dueno)
                .IsUnique();

            modelBuilder.Entity<Paseador>()
                .HasIndex(p => p.correo_paseador)
                .IsUnique();

            modelBuilder.Entity<Manejo_Perfiles>()
                .HasIndex(m => m.nombre_usuario)
                .IsUnique();
        }
    }
}