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
        public int IdRemitente { get; set; }  
        public string? Asunto { get; set; }
        public string? Cuerpo { get; set; }
        public string Remitente { get; set; }
        public int? IdRespuestaMensaje{ get; set; } 
        public DateTime? FechaDeMensaje { get; set; }
        public Clientes Clientes { get; set; }
        public Usuarios Usuarios { get; set; }

    }
}
