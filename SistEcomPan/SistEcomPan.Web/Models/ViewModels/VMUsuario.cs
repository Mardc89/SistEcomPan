namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMUsuario
    {
        public int IdUsuario { get; set; }
        public string? Dni { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Clave { get; set; }
        public int? IdRol { get; set; }
        public string? NombreRol { get; set; }
        public string? UrlFoto { get; set; }
        public string? NombreFoto { get; set; }
        public int? EsActivo { get; set; }
    }
}
