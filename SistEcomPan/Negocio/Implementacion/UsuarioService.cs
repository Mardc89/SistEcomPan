using Datos.Interfaces;
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
        private readonly IUsuarioRepository _usuarios;
        public UsuarioService(IEncriptService encriptservice, IUsuarioRepository usuarios)
        {
            _encriptservice = encriptservice;
            _usuarios=usuarios;
    }

        public Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuarios> Crear(Usuarios entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            try
            {
                string Clave = entidad.Clave;
                entidad.Clave = _encriptservice.ConvertirSha256(Clave);
                entidad.NombreFoto = NombreFoto;

                Usuarios usuarioCreado = await _usuarios.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear Usuario");



            return usuarioCreado;
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        public Task<Usuarios> Editar(Usuarios entidad, Stream Foto = null, string NombreFoto = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GuardarPerfil(Usuarios entidad)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuarios>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<Usuarios> ObtenerPorCredenciales(string correo, string clave)
        {
            throw new NotImplementedException();
        }

        public Task<Usuarios> ObtenerPorId(int IdUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            throw new NotImplementedException();
        }
    }
}
