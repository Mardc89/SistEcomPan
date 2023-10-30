using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Descuentos
    {
        public int IdDescuento { get; set; }
        public int IdProducto { get; set; }
        public Productos Producto { get; set; }
        public decimal Descuento { get; set; }
        public bool Estado { get; set; }
    }
}
