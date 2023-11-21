using Datos.Implementacion;
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
        private readonly IGenericRepository<Usuarios> _repositorio;
        private readonly IEncriptService _encriptservice;
        private readonly ICorreoService _correoService;
        public UsuarioService(IEncriptService encriptservice, ICorreoService correoService, IGenericRepository<Usuarios> repositorio)
        {
            _encriptservice = encriptservice;
            _correoService = correoService;
            _repositorio = repositorio;
        }

        public Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuarios> Crear(Usuarios entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {

            IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
            IQueryable <Usuarios>usuarioEvaluado=usuarios.Where(u => u.Correo == entidad.Correo);
            Usuarios usuarioExiste=usuarioEvaluado.FirstOrDefault();


            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo no Existe");
           
            try
            {
                string Clave = entidad.Clave;
                entidad.Clave = _encriptservice.ConvertirSha256(Clave);
                entidad.NombreFoto = NombreFoto;

                Usuarios usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el Usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]","********");

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

        public async Task<Usuarios> Editar(Usuarios entidad, Stream Foto = null, string NombreFoto = "")
        {

            IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
            IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u => u.Correo == entidad.Correo && u.IdUsuario!=entidad.IdUsuario);
            Usuarios usuarioExiste = usuarioEvaluado.FirstOrDefault();


            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo no Existe");

            try
            {
                IQueryable<Usuarios> buscarUsuario = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEncontrado = buscarUsuario.Where(u =>u.IdUsuario != entidad.IdUsuario);
                Usuarios usuarioEditar = usuarioEncontrado.First();

                usuarioEditar.NombreUsuario = entidad.NombreUsuario;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.IdRol = entidad.IdRol;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

                if (Foto != null){
                    usuarioEditar.UrlFoto =entidad.UrlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo moficar el usuario");

                return usuarioEditar;


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u =>u.IdUsuario==IdUsuario);
                Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado.IdUsuario);

                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> GuardarPerfil(Usuarios entidad)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Usuarios>> Lista()
        {
            List<Usuarios> query = await _repositorio.Lista();
            return query;
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
