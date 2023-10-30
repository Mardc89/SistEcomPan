using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Mensajes
    {
        public int IdMensaje { get; set; }
        public int IdCliente { get; set; }
        public Clientes Cliente { get; set; }
        public string Asunto { get; set; }
        public string Descripcion{ get; set; }
        public DateTime Fecha { get; set; }
    }
}
