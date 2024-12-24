using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AllkuApi.Models;


public class Administrador
{
    [Key]
    [Column("cedula_administrador")]
    [StringLength(10)]
    public string CedulaAdministrador { get; set; }

    [Required]
    [Column("nombre_administrador")]
    [StringLength(50)]
    public string NombreAdministrador { get; set; }

    [Column("usuario_administrador")]
    [StringLength(50)]
    public string UsuarioAdministrador { get; set; }

    [Column("correo_administrador")]
    [StringLength(50)]
    public string CorreoAdministrador { get; set; }

    [Required]
    [Column("contrasena_administrador")]
    [StringLength(50)]
    public string ContrasenaAdministrador { get; set; }

    // Relación con Manejo_Perfiles
    public ICollection<Manejo_Perfiles> Manejo_Perfiles { get; set; }
}


