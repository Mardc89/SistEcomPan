using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public string Dni { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }        
        public string NombreUsuario { get; set; }        
        public string Clave { get; set; }
        public int IdRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }
        public string UrlFoto { get; set; }
        public string NombreFoto { get; set; }
    }
}
