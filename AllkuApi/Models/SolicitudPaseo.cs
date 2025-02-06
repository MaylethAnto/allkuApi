using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class SolicitudPaseo
    {
        [Key]
        [Column("id_solicitud")]
        public int IdSolicitud { get; set; }

        [Required]
        [Column("id_canino")]
        public int IdCanino { get; set; }

        [Required]
        [Column("cedula_paseador")]
        [StringLength(10)]
        public string CedulaPaseador { get; set; }

        [Required]
        [Column("estado_solicitud")]
        [StringLength(20)]
        public string EstadoSolicitud { get; set; }

        [Required]
        [Column("fecha_solicitud")]
        public DateTime FechaSolicitud { get; set; }

        [Column("fecha_respuesta")]
        public DateTime? FechaRespuesta { get; set; }

        // Relación con Canino
        [ForeignKey("IdCanino")]
        public Canino Canino { get; set; }

        // Relación con Paseador
        [ForeignKey("CedulaPaseador")]
        public Paseador Paseador { get; set; }

        // Relación con Paseo
        public ICollection<Paseo> Paseos { get; set; }
    }
}