using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Pagos
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public Pedidos Pedido { get; set; }
        public decimal MontoTotalPedido { get; set; }
        public decimal PagoDelCliente { get; set; }
        public decimal VueltoDelCliente { get; set; }
        public decimal MontoDeuda{ get; set; }
        public DateTime FechaDeuda { get; set; }


    }
}
