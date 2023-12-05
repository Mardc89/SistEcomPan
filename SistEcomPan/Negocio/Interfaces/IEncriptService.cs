using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IEncriptService
    {
        public string GenerarClave();
        public string ConvertirSha256(string texto);
        public string EncriptarPassword(string password);
        public string DesencriptarPassword(string encryptedPassword);



    }
}
