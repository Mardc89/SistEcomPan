using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DetalleDevolucion
    {
        public int IdDetalleDevolucion { get; set; }
        public int IdDevolucion { get; set; }
        public string Categoria { get; set; }
        public string Descripcion { get; set; }       
        public decimal Precio { get; set; }
        public int CantidadPedido { get; set; }        
        public decimal? Total { get; set; }
        public int CantidadDevolucion { get; set; }
        public Devolucion Devolucion { get; set; }
    }
}
