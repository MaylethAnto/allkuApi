using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Ejercicio
    {
        [Key]
        public int id_ejercicio { get; set; }
        public int? id_canino { get; set; }
        public DateTime? fecha_ejercicio { get; set; }
        public int? duracion { get; set; }
        public string? tipo_ejercicio { get; set; }
        public string? nombre_truco { get; set; }
        public byte[]? foto_ejercicio { get; set; }

        [ForeignKey("id_canino")]
        public virtual Canino? Canino { get; set; }
    }
}