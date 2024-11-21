using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AllkuApi.Models;


public class Administrador
{
    [Key]
    [StringLength(10)]
    public string CedulaAdministrador { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreAdministrador { get; set; }

    [StringLength(50)]
    public string UsuarioAdministrador { get; set; }

    [StringLength(50)]
    public string CorreoAdministrador { get; set; }

    [Required]
    [StringLength(50)]
    public string ContrasenaAdministrador { get; set; }

    // Relación con Manejo_Perfiles
    public ICollection<Manejo_Perfiles> ManejoPerfiles { get; set; }
}


