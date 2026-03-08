using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PedidoDTO
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public string? NombresCompletos { get; set; }
        public string Codigo { get; set; }
        public string MontoTotal { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaPedido { get; set; }

    }
}
