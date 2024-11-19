namespace AllkuApi.Dtos
{
    public class RegisterDto
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string TipoPerfil { get; set; }
        public int? IdAdministrador { get; set; }
        public string? CedulaDueno { get; set; }
        public int? IdPaseador { get; set; }
    }
}
