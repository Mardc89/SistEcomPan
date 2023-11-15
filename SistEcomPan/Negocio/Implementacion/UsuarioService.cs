using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IEncriptService _encriptservice;
        public UsuarioService(IEncriptService encriptservice)
        {
            _encriptservice = encriptservice;
        }
        public Task<Usuarios> Crear(Usuarios entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            try
            {
                string Clave = entidad.Clave;
                entidad.Clave = _encriptservice.ConvertirSha256(Clave);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<Usuarios>> Lista()
        {
            throw new NotImplementedException();
        }
    }
}
