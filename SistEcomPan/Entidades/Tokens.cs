using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Tokens
    {
        public int IdToken { get; set; }
        public string Perfil { get; set; }
        public int IdPerfil { get; set; }
        public string Token { get; set;}
        public DateTime Expiracion { get; set; }
        public DateTime Creacion{ get; set; }

    }
}
