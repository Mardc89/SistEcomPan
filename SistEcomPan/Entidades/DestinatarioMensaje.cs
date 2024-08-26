using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DestinatarioMensaje
    {
        public int IdMensaje { get; set; }
        public int IdDestinatario { get; set; }
        public string? Destinatario { get; set; }
    }
}
