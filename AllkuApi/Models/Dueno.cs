using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class Dueno
    {
        [Key]
        public string cedula_dueno { get; set; } = null!;
        public string? nombre_dueno { get; set; }
        public string? direccion_dueno { get; set; }
        public string? celular_dueno { get; set; }
        public string? correo_dueno { get; set; }
    }
}