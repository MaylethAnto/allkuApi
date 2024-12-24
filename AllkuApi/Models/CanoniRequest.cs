using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class CaninoRequest
    {
        [Required]
        [StringLength(50)]
        public string NombreCanino { get; set; }

        [Required]
        public int EdadCanino { get; set; }

        [Required]
        [StringLength(50)]
        public string RazaCanino { get; set; }

        [Required]
        public decimal PesoCanino { get; set; }

        [Required]
        public byte[] FotoCanino { get; set; }

        [Required]
        public string CedulaDueno { get; set; }
    }
}