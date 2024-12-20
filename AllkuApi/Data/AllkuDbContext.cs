﻿using Microsoft.EntityFrameworkCore;
using AllkuApi.Models;

namespace AllkuApi.Data
{
    public class AllkuDbContext : DbContext
    {
        public AllkuDbContext(DbContextOptions<AllkuDbContext> options) : base(options) { }

        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<Canino> Canino { get; set; }
        public DbSet<Dueno> Dueno { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<GPS> GPS { get; set; }
        public DbSet<Historial_Clinico> Historial_Clinico { get; set; }
        public DbSet<Manejo_Perfiles> Manejo_Perfiles { get; set; }
        public DbSet<Paseador> Paseador { get; set; }
        public DbSet<Receta> Receta { get; set; }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().ToTable("Administrador");
            modelBuilder.Entity<Manejo_Perfiles>().ToTable("Manejo_Perfiles");
            modelBuilder.Entity<Canino>().ToTable("Canino");
            modelBuilder.Entity<Canino>()
           .HasOne<Dueno>()  // Relación con Dueno, pero solo a través de la cédula
           .WithMany(d => d.Caninos)  // Relación inversa en Dueno (Caninos)
           .HasForeignKey(c => c.CedulaDueno)  // La clave foránea es CedulaDueno en Canino
           .OnDelete(DeleteBehavior.Cascade);
        }
        // Método para iniciar sesión (buscar en Manejo_Perfiles por nombre de usuario y contraseña)
        public async Task<Manejo_Perfiles> IniciarSesion(string nombreUsuario, string contrasena)
        {
            return await Manejo_Perfiles
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);
        }
    }

}