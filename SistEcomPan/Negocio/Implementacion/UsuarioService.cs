﻿using Datos.Implementacion;
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
      
        public UsuarioService(IEncriptService encriptservice, ICorreoService correoService, IGenericRepository<Usuarios> repositorio,IGenericRepository<Clientes> repositorioCliente)
        {
            _encriptservice = encriptservice;
            _correoService = correoService;
            _repositorio = repositorio;
            _repositorioCliente = repositorioCliente;
           
        }

        public async Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuarios usuarioEncontrado = await _repositorio.Buscar(null,null,IdUsuario);

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

            Usuarios usuarioExiste = await _repositorio.Buscar(entidad.Correo,null,null);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo ya Existe");
           
            try
            {
                entidad.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null && Foto.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","ImagenesPerfil");
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

            Usuarios usuarioExiste = await _repositorio.Verificar(entidad.Correo, null, entidad.IdUsuario);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El Correo ya Existe");

            try
            {

                Usuarios usuarioEditar = await _repositorio.Buscar(null, null, entidad.IdUsuario);

                usuarioEditar.Dni = entidad.Dni;
                usuarioEditar.Nombres = entidad.Nombres;
                usuarioEditar.Apellidos = entidad.Apellidos;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.NombreUsuario = entidad.NombreUsuario;
                usuarioEditar.Clave = _encriptservice.EncriptarPassword(entidad.Clave);
                usuarioEditar.IdRol = entidad.IdRol;
                usuarioEditar.Estado = entidad.Estado;

                if (usuarioEditar.NombreFoto == "" || NombreFoto!="")
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

                Usuarios usuarioEncontrado = await _repositorio.Buscar(null, null, IdUsuario);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado.IdUsuario);

                if (nombreFoto!="")
                    System.IO.File.Delete(usuarioEncontrado.UrlFoto);

                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> GuardarPerfil(Usuarios entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {

                Usuarios usuarioEncontrado = await _repositorio.Buscar(null, null, entidad.IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El Usuario no Existe");
           
                usuarioEncontrado.Dni = entidad.Dni;
                usuarioEncontrado.Nombres = entidad.Nombres;
                usuarioEncontrado.Apellidos = entidad.Apellidos;
                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.IdRol = entidad.IdRol;
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

        public async Task<List<Usuarios>> Lista()
        {
            List<Usuarios> query = await _repositorio.Lista();
            return query;
        }

        public async Task<string> ObtenerNombreCompleto(int IdUsuario)
        {
            Usuarios usuarios = await _repositorio.Buscar(null, null, IdUsuario);
            return usuarios.Apellidos + " " + usuarios.Nombres;
        }
        public async Task<string> ObtenerCorreo(int IdUsuario)
        {
            Usuarios usuarios = await _repositorio.Buscar(null, null, IdUsuario);
            return usuarios.Correo;
        }
        public async Task<int> ObtenerIdCorreoUsuario(string correo)
        {

            try
            {
                Usuarios usuarios = await _repositorio.Buscar(correo, null, null);

                return usuarios?.IdUsuario??0;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Usuarios> ObtenerUsuario(string correo)
        {

            try
            {
                Usuarios usuarios = await _repositorio.Buscar(correo, null, null);

                return usuarios;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Usuarios> ObtenerPorCredenciales(string correo, string clave)
        {
            string ClaveEncriptada = _encriptservice.EncriptarPassword(clave);
            Usuarios usuarioEncontrado = await _repositorio.Buscar(correo, ClaveEncriptada, null);
            return usuarioEncontrado;
            
        }

        public async Task<Usuarios> ObtenerPorId(int IdUsuario)
        {

            Usuarios usuarioEncontrado = await _repositorio.Buscar(null, null, IdUsuario);

            return usuarioEncontrado;
        }

        public async Task<bool> RestablecerClave(string ClaveNueva, string Correo, string UrlPlantillaCorreo)
        {
            try
            {

                Usuarios usuarioEncontrado = await _repositorio.Buscar(Correo);

                Clientes clienteEncontrado = await _repositorioCliente.Buscar(Correo);

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

        public Task<int> IdDestinatario(string correo)
        {
            throw new NotImplementedException();
        }

        public Task<int> IdRemitente(string correo)
        {
            throw new NotImplementedException();
        }
    }
}
