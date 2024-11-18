using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class GPS
    {
        [Key]
        public int id_gps { get; set; }
        public int? id_canino { get; set; }
        public DateTime? fecha_gps { get; set; }
        public string? ubicacion_gps { get; set; }
        public int? pasos_gps { get; set; }
        public decimal? distancia_km { get; set; }

        [ForeignKey("id_canino")]
        public virtual Canino? Canino { get; set; }
    }
}