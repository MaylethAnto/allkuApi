using AllkuApi.Models;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models { 
public class Dueno
{
    [Key]
    [StringLength(10)]
    public string CedulaDueno { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreDueno { get; set; }

    [StringLength(100)]
    public string DireccionDueno { get; set; }

    [StringLength(15)]
    public string CelularDueno { get; set; }

    [StringLength(50)]
    public string CorreoDueno { get; set; }

    // Relación con Canino
    public ICollection<Canino> Caninos { get; set; }
    }
}
