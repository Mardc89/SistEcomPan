using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DetallePago
    {
        public int IdDetallePago { get; set; }
        public int IdPago { get; set; }
        public Pagos Pagos { get; set; }
        public decimal  MontoAPagar{ get; set; }
        public decimal PagoDelCliente { get; set; }
        public decimal DeudaDelCliente { get; set; }
        public decimal CambioDelCliente { get; set; }
    }
}
