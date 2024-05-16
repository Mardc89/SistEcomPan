using Entidades;

namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMCliente
    {
        public int IdCliente { get; set; }
        public string TipoCliente { get; set; }
        public string Dni { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public int IdDistrito { get; set; }
        public string NombreDistrito { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public int? Estado { get; set; }
        public string? UrlFoto { get; set; }
        public string? NombreFoto { get; set; }
    }
}
