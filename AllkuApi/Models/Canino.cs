using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Canino
    {
        [Key]
        public int IdCanino { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCanino { get; set; }

        public int EdadCanino { get; set; }

        [StringLength(50)]
        public string RazaCanino { get; set; }

        public decimal PesoCanino { get; set; }

        public byte[] FotoCanino { get; set; }

        // Relación con Dueno
        public string CedulaDueno { get; set; }
        public Dueno Dueno { get; set; }
    }

}