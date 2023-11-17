using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IEncriptService _encriptservice;
        private readonly IUsuarioRepository _usuarios;
        private readonly ICorreoService _correoService;
        public UsuarioService(IEncriptService encriptservice, IUsuarioRepository usuarios, ICorreoService correoService)
        {
            _encriptservice = encriptservice;
            _usuarios = usuarios;
            _correoService = correoService;
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
                    throw new TaskCanceledException("No se pudo crear el Usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]","");

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();

                        }


                    }

                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(usuarioCreado.Correo, "Cuenta Creada", htmlCorreo);
                }

               
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
