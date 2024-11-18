using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Historial_Clinico
    {
        [Key]
        public int id_historial { get; set; }
        public int? id_canino { get; set; }
        public DateTime? fecha_historial { get; set; }
        public string? tipo_historial { get; set; }
        public string? descripcion_historial { get; set; }
        public bool? notificacion_historial { get; set; }

        [ForeignKey("id_canino")]
        public virtual Canino? Canino { get; set; }
    }
}