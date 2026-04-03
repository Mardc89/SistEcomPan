using Entidades;
using System.ComponentModel.DataAnnotations;

namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMCliente
    {

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Teléfono no válido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Seleccione tipo de cliente")]
        public string TipoCliente { get; set; }

        [Required(ErrorMessage = "Seleccione un distrito")]
        public int IdDistrito { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Clave { get; set; }
        public int IdCliente { get; set; }
        public string? NombreCompleto { get; set; }
        public string? NombreDistrito { get; set; }
        public int? Estado { get; set; }
        public string? UrlFoto { get; set; }
        public string? NombreFoto { get; set; }
    }
}
