using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Paseador
    {
        [Key]
        public int id_paseador { get; set; }
        public string? nombre_paseador { get; set; }
        public string? apellido_paseador { get; set; }
        public string? direccion_paseador { get; set; }
        public string? celular_paseador { get; set; }
        public string? correo_paseador { get; set; }
        public int? id_canino { get; set; }

        [ForeignKey("id_canino")]
        public virtual Canino? Canino { get; set; }
    }
}