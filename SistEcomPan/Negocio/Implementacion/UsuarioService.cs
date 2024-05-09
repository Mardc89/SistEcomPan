using Datos.Implementacion;
using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Hosting;
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
        private readonly IGenericRepository<Clientes> _repositorioCliente;
        private readonly IEncriptService _encriptservice;
        private readonly ICorreoService _correoService;
        private readonly IHostEnvironment _environment;
        public UsuarioService(IEncriptService encriptservice, ICorreoService correoService, IGenericRepository<Usuarios> repositorio,IHostEnvironment environment)
        {
            _encriptservice = encriptservice;
            _correoService = correoService;
            _repositorio = repositorio;
            _environment = environment;
        }

        public async Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            try
            {
                IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u => u.IdUsuario == IdUsuario);
                Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");
                if (usuarioEncontrado.Clave != _encriptservice.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña ingresada como actual no es correcta");

                usuarioEncontrado.Clave = _encriptservice.ConvertirSha256(claveNueva);
                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                entidad.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Imagenes");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path,NombreFoto);

                    string UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        Foto.CopyTo(stream);

                    }
                    entidad.UrlFoto = UrlFoto;
                }

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
                throw new TaskCanceledException("El Correo ya Existe");

            try
            {
                IQueryable<Usuarios> buscarUsuario = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEncontrado = buscarUsuario.Where(u =>u.IdUsuario == entidad.IdUsuario);
                Usuarios usuarioEditar = usuarioEncontrado.First();

                usuarioEditar.Dni = entidad.Dni;
                usuarioEditar.Nombres = entidad.Nombres;
                usuarioEditar.Apellidos = entidad.Apellidos;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.NombreUsuario = entidad.NombreUsuario;
                usuarioEditar.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                usuarioEditar.IdRol = entidad.IdRol;
                usuarioEditar.Estado = entidad.Estado;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagenes");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path, NombreFoto);

                    string UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        Foto.CopyTo(stream);

                    }
                    usuarioEditar.UrlFoto = UrlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el usuario");

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

                if (respuesta)
                    System.IO.File.Delete(usuarioEncontrado.UrlFoto);

                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> GuardarPerfil(Usuarios entidad)
        {
            try
            {
                IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u =>u.IdUsuario==entidad.IdUsuario);
                Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Nombres = entidad.Nombres;
                
                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Usuarios>> Lista()
        {
            List<Usuarios> query = await _repositorio.Lista();
            return query;
        }

        public async Task<Usuarios> ObtenerPorCredenciales(string correo, string clave)
        {
            string ClaveEncriptada = _encriptservice.EncriptarPassword(clave);
            IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
            IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u => u.Correo.Equals(correo)&&u.Clave.Equals(ClaveEncriptada));
            Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

            return usuarioEncontrado;
            
        }

        public async Task<Usuarios> ObtenerPorId(int IdUsuario)
        {

            IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
            IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u => u.IdUsuario==IdUsuario);
            Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

            return usuarioEncontrado;
        }

        public async Task<bool> RestablecerClave(string ClaveNueva, string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                IQueryable<Usuarios> usuarios = await _repositorio.Consultar();
                IQueryable<Usuarios> usuarioEvaluado = usuarios.Where(u => u.Correo == Correo);
                Usuarios usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                IQueryable<Clientes> clientes = await _repositorioCliente.Consultar();
                IQueryable<Clientes> clienteEvaluado = clientes.Where(u => u.Correo == Correo);
                Clientes clienteEncontrado = clienteEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null && clienteEncontrado==null)
                    throw new TaskCanceledException("El Usuario no Existe");

                if (usuarioEncontrado !=null) {
                    usuarioEncontrado.Clave = _encriptservice.EncriptarPassword(ClaveNueva);
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioEncontrado.Correo).Replace("[clave]", "********");
                }
                else
                {
                    clienteEncontrado.Clave = _encriptservice.EncriptarPassword(ClaveNueva);
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", clienteEncontrado.Correo).Replace("[clave]", "********");
                }
                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK){

                   using (Stream dataStream = response.GetResponseStream()){
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

                bool correoEnviado = false;
                bool respuesta = false;

                if (htmlCorreo != "")
                    correoEnviado= await _correoService.EnviarCorreo(Correo, "Contraseña Restablecida", htmlCorreo);

                if (!correoEnviado)
                    throw new TaskCanceledException("Tenemos problemas.Por Favor inténtalo de nuevo mas tarde");
                if (usuarioEncontrado != null) {
                    respuesta = await _repositorio.Editar(usuarioEncontrado);
                    
                }
                else if(clienteEncontrado != null)
                {
                    respuesta = await _repositorioCliente.Editar(clienteEncontrado);
                    
                }

                return respuesta;
                


            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
