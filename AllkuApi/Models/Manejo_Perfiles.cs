using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Manejo_Perfiles
    {
        [Key]
        public int IdPerfil { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Contrasena { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public bool EstadoCuenta { get; set; } = true;

        [Required]
        [StringLength(20)]
        public string RolUsuario { get; set; }

        public DateTime? UltimoInicioSesion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public string CedulaAdministrador { get; set; }
        public Administrador Administrador { get; set; }

        public string CedulaDueno { get; set; }
        public Dueno Dueno { get; set; }

        public string CedulaPaseador { get; set; }
        public Paseador Paseador { get; set; }
    }

}