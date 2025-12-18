using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Productos
    {
        public int IdProducto { get; set;}
        public string Descripcion { get; set; }
        public int IdCategoria { get; set;}
        public Categorias Categoria { get; set; }
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; }
        public string NombreImagen { get; set; }
        public bool Estado { get; set; }
        public int Stock { get; set; }
        public DateTime? FechaRegistro { get; set; }


    }
}
