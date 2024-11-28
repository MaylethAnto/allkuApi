using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Canino
    {
        [Key]
        [Column("id_canino")]
        public int IdCanino { get; set; }

        [Required]
        [Column("nombre_canino")]
        [StringLength(50)]
        public string NombreCanino { get; set; }

        [Required]
        [Column("edad_canino")]
        public int EdadCanino { get; set; }

        [Required]
        [Column("raza_canino")]
        [StringLength(50)]
        public string RazaCanino { get; set; }

        [Required]
        [Column("peso_canino")]
        public decimal PesoCanino { get; set; }

        [Required]
        [Column("foto_canino")]
        public byte[] FotoCanino { get; set; }

        // Relación con Dueno
        [ForeignKey("Dueno")]
        [Column("cedula_dueno")]
        public string CedulaDueno { get; set; }
       
    }

}