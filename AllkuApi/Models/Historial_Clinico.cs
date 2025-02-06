using AllkuApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    [Table("Historial_Clinico")]
    public class Historial_Clinico
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

        [Column("id_canino")]
        public int IdCanino { get; set; }

        [Column("fecha_historial")]
        public DateTime FechaHistorial { get; set; }

        [Column("tipo_historial")]
        public string TipoHistorial { get; set; }

        [Column("descripcion_historial")]
        public string DescripcionHistorial { get; set; }

        // Propiedad de navegación
        [ForeignKey("IdCanino")]
        public Canino Canino { get; set; }
    }
}