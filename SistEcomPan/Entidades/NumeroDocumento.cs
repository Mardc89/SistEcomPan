using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class NumeroDocumento
    {
        public int IdNumeroDocumento { get; set; }
        public int UltimoNumero { get; set; }
        public int CantidadDeDigitos { get; set; }
        public string Gestion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
