using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class Administrador
    {
        [Key]
        public int id_administrador { get; set; }
        public string? nombre_administrador { get; set; }
        public string? usuario_administrador { get; set; }
        public string? correo_administrador { get; set; }
        public string? contrasena_administrador { get; set; }
    }
}