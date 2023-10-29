using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int Pedido { get; set; }
        public Productos Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
