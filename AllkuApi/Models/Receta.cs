using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Receta
    {
        [Key]
        public int id_receta { get; set; }
        public string? nombre_receta { get; set; }
        public string? descripcion_receta { get; set; }
        public byte[]? foto_receta { get; set; }
        public int? id_canino { get; set; }

        [ForeignKey("id_canino")]
        public virtual Canino? Canino { get; set; }
    }
}