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
        private readonly IClienteRepository _repositorio;
        private readonly IEncriptService _encriptservice;
        private readonly ICorreoService _correoService;
        public ClienteService(IEncriptService encriptservice, ICorreoService correoService, IClienteRepository repositorio)
        {
            _encriptservice = encriptservice;
            _correoService = correoService;
            _repositorio = repositorio;
 
        }
        public async Task<string> ObtenerNombreCompleto(int IdCliente)
        {
            Clientes clientes = await _repositorio.Buscar(null,null,IdCliente);
            return clientes.Apellidos +" "+ clientes.Nombres;
        }

        public async Task<string> ObtenerCorreo(int IdCliente)
        {
            Clientes clientes = await _repositorio.Buscar(null, null, IdCliente);
            return clientes.Correo;
        }
        public async Task<int> ObtenerIdCorreoCliente(string correo)
        {

            try
            {
                Clientes clientes = await _repositorio.Buscar(correo, null, null);
                return clientes?.IdCliente??0;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Clientes> ObtenerCliente(string correo)
        {

            try
            {
                Clientes clientes = await _repositorio.Buscar(correo, null, null);
                return clientes;

            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<string> ObtenerDni(int IdCliente)
        {
            Clientes clientes = await _repositorio.Buscar(null, null, IdCliente);
            return clientes.Dni;
        }
        public async Task<List<Clientes>> Lista()
        {
            List<Clientes> query = await _repositorio.Lista();
            return query;
        }

        public async Task<List<Clientes>> ObtenerNombre()
        {
            List<Clientes> lista = await _repositorio.Lista();
            return lista;
        }

        public async Task<Clientes> Crear(Clientes entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {

            Clientes usuarioExiste = await _repositorio.Buscar(entidad.Correo,null,null);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo ya Existe");

            try
            {
                string Clave = entidad.Clave;
                entidad.Clave = _encriptservice.EncriptarPassword(Clave);
                entidad.NombreFoto = NombreFoto;

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

            Clientes usuarioExiste = await _repositorio.Verificar(entidad.Correo,null, entidad.IdCliente);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo ya Existe");

            try
            {
                Clientes usuarioEditar = await _repositorio.Buscar(null,null, entidad.IdCliente);

                usuarioEditar.NombreUsuario = entidad.NombreUsuario;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Nombres = entidad.Nombres;
                usuarioEditar.Dni = entidad.Dni;
                usuarioEditar.Apellidos = entidad.Apellidos;
                usuarioEditar.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                usuarioEditar.TipoCliente = entidad.TipoCliente;
                usuarioEditar.Estado = entidad.Estado;
                usuarioEditar.Telefono = entidad.Telefono;
                usuarioEditar.Direccion = entidad.Direccion;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

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
                    usuarioEditar.UrlFoto = UrlFoto;
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
                Clientes usuarioEncontrado = await _repositorio.Buscar(null,null,IdCliente);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado.IdCliente);

                if (nombreFoto != "")
                    System.IO.File.Delete(usuarioEncontrado.UrlFoto);

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
            Clientes usuarioEncontrado = await _repositorio.Buscar(correo,ClaveEncriptada,null);
            return usuarioEncontrado;

        }

        public async Task<Clientes> ObtenerPorId(int IdCliente)
        {
            Clientes cliente = await _repositorio.Buscar(null,null,IdCliente);

            return cliente;
        }

        public async Task<bool> GuardarPerfil(Clientes entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {

                Clientes usuarioEncontrado = await _repositorio.Buscar(null,null,entidad.IdCliente);

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
                Clientes usuarioEncontrado = await _repositorio.Buscar(null,null,IdCliente);

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

                Clientes usuarioEncontrado = await _repositorio.Buscar(Correo);

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

        public async Task<List<Clientes>> ClienteFiltrado(string busqueda)
        {
            List<Clientes> query = await _repositorio.Lista();

            var ClienteFiltrado = query.Where(p =>
            string.IsNullOrWhiteSpace(busqueda) || p.Apellidos.ToLower().EndsWith(busqueda.ToLower()) ||
            p.Apellidos.ToLower().StartsWith(busqueda.ToLower()) ||
            p.Nombres.ToLower().StartsWith(busqueda.ToLower()) ||
            p.Nombres.ToLower().EndsWith(busqueda.ToLower()) ||
            p.Dni.ToLower().Contains(busqueda.ToLower())
            );

            return ClienteFiltrado.ToList();

        }


        public async Task<Clientes?> SepararApellidosNombres(string apellidos, string nombres)
        {
            return await _repositorio.Buscar(null,null,null,apellidos,nombres);


        }
    }
}
