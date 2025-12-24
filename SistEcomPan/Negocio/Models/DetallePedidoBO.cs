using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Models
{
    public class DetallePedidoBO
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal Precio { get; set; }
        public int CantidadTotal { get; set; }
        public decimal? Total { get; set; }
    }
}
