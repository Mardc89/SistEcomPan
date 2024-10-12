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
        public string Codigo { get; set; }
        public decimal? MontoTotal { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public List<DetalleDevolucion> DetalleDevolucion { get; set; }
    }
}
