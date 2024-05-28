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
    public class ClienteService:IClienteService
    {
        private readonly IGenericRepository<Clientes> _repositorio;
        private readonly IEncriptService _encriptservice;
        private readonly ICorreoService _correoService;
        public ClienteService(IEncriptService encriptservice, ICorreoService correoService, IGenericRepository<Clientes> repositorio)
        {
            _encriptservice = encriptservice;
            _correoService = correoService;
            _repositorio = repositorio;
        }

        public async Task<List<Clientes>> Lista()
        {
            List<Clientes> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Clientes>> ObtenerNombre()
        {
            List<Clientes> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }

        public async Task<Clientes> Crear(Clientes entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {

            IQueryable<Clientes> clientes = await _repositorio.Consultar();
            IQueryable<Clientes> usuarioEvaluado = clientes.Where(u => u.Correo == entidad.Correo);
            Clientes usuarioExiste = usuarioEvaluado.FirstOrDefault();


            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo no Existe");

            try
            {
                string Clave = entidad.Clave;
                entidad.Clave = _encriptservice.EncriptarPassword(Clave);
                entidad.NombreFoto = NombreFoto;

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
                    entidad.UrlFoto = UrlFoto;
                }

                Clientes usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado.IdCliente == 0)
                    throw new TaskCanceledException("No se pudo crear el Usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", "********");

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


        public async Task<Clientes> Editar(Clientes entidad, Stream Foto = null, string NombreFoto = "")
        {

            IQueryable<Clientes> clientes = await _repositorio.Consultar();
            IQueryable<Clientes> usuarioEvaluado = clientes.Where(u => u.Correo == entidad.Correo && u.IdCliente != entidad.IdCliente);
            Clientes usuarioExiste = usuarioEvaluado.FirstOrDefault();


            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo no Existe");

            try
            {
                IQueryable<Clientes> buscarUsuario = await _repositorio.Consultar();
                IQueryable<Clientes> usuarioEncontrado = buscarUsuario.Where(u => u.IdCliente != entidad.IdCliente);
                Clientes usuarioEditar = usuarioEncontrado.First();

                usuarioEditar.NombreUsuario = entidad.NombreUsuario;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Nombres = entidad.Nombres;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    usuarioEditar.UrlFoto = entidad.UrlFoto;
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


        public async Task<bool> Eliminar(int IdCliente)
        {
            try
            {
                IQueryable<Clientes> usuarios = await _repositorio.Consultar();
                IQueryable<Clientes> usuarioEvaluado = usuarios.Where(u => u.IdCliente == IdCliente);
                Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado.IdCliente);

                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Clientes> ObtenerPorCredenciales(string correo, string clave)
        {
            string ClaveEncriptada = _encriptservice.EncriptarPassword(clave);
            IQueryable<Clientes> usuarios = await _repositorio.Consultar();
            IQueryable<Clientes> usuarioEvaluado = usuarios.Where(u => u.Correo.Equals(correo) && u.Clave.Equals(ClaveEncriptada));
            Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

            return usuarioEncontrado;

        }

        public async Task<Clientes> ObtenerPorId(int IdCliente)
        {

            IQueryable<Clientes> usuarios = await _repositorio.Consultar();
            IQueryable<Clientes> usuarioEvaluado = usuarios.Where(u => u.IdCliente == IdCliente);
            Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

            return usuarioEncontrado;
        }

        public async Task<bool> GuardarPerfil(Clientes entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                IQueryable<Clientes> usuarios = await _repositorio.Consultar();
                IQueryable<Clientes> usuarioEvaluado = usuarios.Where(u => u.IdCliente == entidad.IdCliente);
                Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");
            
                usuarioEncontrado.TipoCliente = entidad.TipoCliente;   
                usuarioEncontrado.Dni = entidad.Dni;
                usuarioEncontrado.Nombres = entidad.Nombres;
                usuarioEncontrado.Apellidos = entidad.Apellidos;
                usuarioEncontrado.Correo = entidad.Correo;     
                usuarioEncontrado.Direccion = entidad.Direccion;
                usuarioEncontrado.Telefono = entidad.Telefono;
                usuarioEncontrado.IdDistrito = entidad.IdDistrito;   
                usuarioEncontrado.NombreUsuario = entidad.NombreUsuario;
                usuarioEncontrado.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                usuarioEncontrado.Estado = true;

                if (usuarioEncontrado.NombreFoto == "" || NombreFoto!="")
                    usuarioEncontrado.NombreFoto = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagenesPerfil");
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
                    usuarioEncontrado.UrlFoto = UrlFoto;
                }


                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> CambiarClave(int IdCliente, string claveActual, string claveNueva)
        {
            try
            {
                IQueryable<Clientes> clientes = await _repositorio.Consultar();
                IQueryable<Clientes> usuarioEvaluado = clientes.Where(u => u.IdCliente == IdCliente);
                Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

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


        public async Task<bool> RestablecerClave(string ClaveNueva, string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                IQueryable<Clientes> usuarios = await _repositorio.Consultar();
                IQueryable<Clientes> usuarioEvaluado = usuarios.Where(u => u.Correo == Correo);
                Clientes usuarioEncontrado = usuarioEvaluado.FirstOrDefault();

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                usuarioEncontrado.Clave = _encriptservice.ConvertirSha256(ClaveNueva);


                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioEncontrado.Correo).Replace("[clave]", "********");

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

                bool correoEnviado = false;

                if (htmlCorreo != "")
                    correoEnviado = await _correoService.EnviarCorreo(Correo, "Contraseña Restablecida", htmlCorreo);

                if (!correoEnviado)
                    throw new TaskCanceledException("Tenemos problemas.Por Favor inténtalo de nuevo mas tarde");

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;


            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
