using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class Paseo
    {
        [Key]
        [Column("id_paseo")]
        public int IdPaseo { get; set; }

        [Required]
        [Column("id_solicitud")]
        public int IdSolicitud { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Required]
        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Required]
        [Column("distancia_km", TypeName = "decimal(5,2)")]
        public decimal? DistanciaKm { get; set; }

        [Required]
        [Column("estado_paseo")]
        [StringLength(20)]
        public string EstadoPaseo { get; set; }

        // Relación con SolicitudPaseo
        [ForeignKey("IdSolicitud")]
        public SolicitudPaseo SolicitudPaseo { get; set; }
    }
}
