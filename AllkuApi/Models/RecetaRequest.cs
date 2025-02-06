using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class RecetaRequest
    {
        [Required]
        public int id_receta { get; set; }
        [Required]
        public string? nombre_receta { get; set; }
        [Required]
        public string? descripcion_receta { get; set; }
        [Required]
        public IFormFile? foto_receta { get; set; }
        [Required]
        public int? id_canino { get; set; }
    }
}