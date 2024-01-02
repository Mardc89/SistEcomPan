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
        public decimal MontoDePedido { get; set; }
        public decimal Descuento { get; set; }
        public decimal MontoTotalDePago { get; set; }
        public decimal MontoDeuda{ get; set; }
        public DateTime FechaDePago { get; set; }
        public string Estado { get; set; }


    }
}
