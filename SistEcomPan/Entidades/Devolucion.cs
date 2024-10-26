using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Devolucion
    {
        public int IdDevolucion { get; set; }
        public int IdCliente { get; set; }
        public string CodigoPedido { get; set; }
        public string CodigoDevolucion { get; set; }
        public decimal? MontoPedido { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? MontoAPagar { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public List<DetalleDevolucion> DetalleDevolucion { get; set; }
    }
}
