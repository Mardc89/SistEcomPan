using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ProductoDTO
    {

        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public string TipoDeCategoria { get; set; }
        public decimal Precio { get; set; }
        public string NombreImagen { get; set; }
        public int Stock { get; set; }
    }
}
