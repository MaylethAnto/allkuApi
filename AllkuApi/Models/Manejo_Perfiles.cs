using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AllkuApi.Models
{
    public class Manejo_Perfiles
    {
        [Key]
        [Column("id_perfil")]
        public int IdPerfil { get; set; }

        [Column("nombre_usuario")]
        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Column("contrasena")]
        [Required]
        [StringLength(1000)]
        public string Contrasena { get; set; }

        [Column("intentos_fallidos")]
        public int IntentosFallidos { get; set; } = 0;

        [Column("estado_cuenta")]
        public bool EstadoCuenta { get; set; } = true;

        [Column("rol_usuario")]
        [Required]
        [StringLength(20)]
        public string RolUsuario { get; set; }

        [Column("ultimo_inicio_sesion")]
        public DateTime? UltimoInicioSesion { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("cedula_administrador")]
        public string CedulaAdministrador { get; set; }

        [Column("cedula_dueno")]
        public string CedulaDueno { get; set; }

        [Column("cedula_paseador")]
        public string CedulaPaseador { get; set; }

        // Propiedades de navegación
        public virtual Administrador Administrador { get; set; }
        public virtual Dueno Dueno { get; set; }
        public virtual Paseador Paseador { get; set; }
    }
}