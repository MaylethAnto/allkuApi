using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Canino
    {
        [Key]
        public int id_canino { get; set; }
        public string? nombre_canino { get; set; }
        public int? edad_canino { get; set; }
        public string? raza_canino { get; set; }
        public decimal? peso_canino { get; set; }
        public byte[]? foto_canino { get; set; }
        public string? cedula_dueno { get; set; }
        
        [ForeignKey("cedula_dueno")]
        public virtual Dueno? Dueno { get; set; }
    }
}