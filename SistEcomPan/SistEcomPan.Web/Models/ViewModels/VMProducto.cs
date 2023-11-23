using Entidades;

namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMProducto
    {
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; }
        public int? Estado { get; set; }
        public int Stock { get; set; }
    }
}
