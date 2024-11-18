using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class Manejo_Perfiles
    {
        [Key]
        public int id_perfil { get; set; }
        public string? nombre_usuario { get; set; }
        public string? contrasena { get; set; }
        public string? tipo_perfil { get; set; }
        public int? id_administrador { get; set; }
        public string? cedula_dueno { get; set; }
        public int? id_paseador { get; set; }

        [ForeignKey("id_administrador")]
        public virtual Administrador? Administrador { get; set; }

        [ForeignKey("cedula_dueno")]
        public virtual Dueno? Dueno { get; set; }

        [ForeignKey("id_paseador")]
        public virtual Paseador? Paseador { get; set; }
    }
}