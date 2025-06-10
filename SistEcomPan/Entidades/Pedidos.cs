using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Pedidos
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public Clientes Cliente { get; set;}
        public string Codigo { get; set; }
        public decimal? MontoTotal { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaPedido { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public List<DetallePedido> DetallePedido { get; set; }
    }
}
