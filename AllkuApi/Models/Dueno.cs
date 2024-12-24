using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Dueno
    {
        [Key]
        [Column("cedula_dueno")]
        [StringLength(10)]
        public string CedulaDueno { get; set; }

        [Column("nombre_dueno")]
        [Required]
        [StringLength(50)]
        public string NombreDueno { get; set; }

        [Column("direccion_dueno")]
        [StringLength(100)]
        public string DireccionDueno { get; set; }

        [Column("celular_dueno")]
        [StringLength(15)]
        public string CelularDueno { get; set; }

        [Column("correo_dueno")]
        [StringLength(50)]
        public string CorreoDueno { get; set; }

        // Lista de caninos que posee el dueño
        public ICollection<Canino> Caninos { get; set; }

        // Lista de manejo perfiles
        public ICollection<Manejo_Perfiles> Manejo_Perfiles { get; set; }
    }
}