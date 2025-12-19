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
    public class MensajeService : IMensajeService
    {
        private readonly IDestinatarioNew _repositorio;
        private readonly IClienteService _repositorioCliente;
        private readonly IUsuarioService _repositorioUsuario;
        private readonly IDestinatarioMensajeService _destinatarioMensajeService;

        public MensajeService(IDestinatarioNew repositorio, IClienteService repositorioCliente, IUsuarioService repositorioUsuario, IDestinatarioMensajeService destinatarioMensajeService)
        {
            _repositorio = repositorio;
            _repositorioCliente = repositorioCliente;
            _repositorioUsuario = repositorioUsuario;
            _destinatarioMensajeService = destinatarioMensajeService;
        }

        public async Task<string> Destinatario(string correo)
        {
            string destinatario = "";
            int idCliente = await _repositorioCliente.ObtenerIdCorreoCliente(correo);
            int idUsuario = await _repositorioUsuario.ObtenerIdCorreoUsuario(correo);
            if (idCliente!=0)
            {
                destinatario = "Cliente";
            }
            else if(idUsuario!=0)
            {
                destinatario = "Usuario";

            }
     
            return destinatario;
        }

        public async Task<string> Remitente(string correo)
        {
            string remitente = "";
            int idCliente = await _repositorioCliente.ObtenerIdCorreoCliente(correo);
            int idUsuario = await _repositorioUsuario.ObtenerIdCorreoUsuario(correo);
            if (idCliente != 0)
            {
                remitente = "Cliente";
            }
            else if (idUsuario != 0)
            {
                remitente = "Usuario";

            }

            return remitente;
        }

        public async Task<Mensajes> Editar(Mensajes entidad)
        {

            Mensajes mensajeExiste = await _repositorio.Verificar(null, entidad.Asunto, entidad.IdMensaje);

            if (mensajeExiste != null)
                throw new TaskCanceledException("El Mensaje ya Existe");

            try
            {
                Mensajes mensajeEditar = await _repositorio.Buscar(null, null, entidad.IdMensaje);
                mensajeEditar.Cuerpo= entidad.Cuerpo;

                bool respuesta = await _repositorio.Editar(mensajeEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el mensaje");

                return mensajeEditar;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int IdMensaje)
        {
            try
            {

                Mensajes mensajeEncontrado = await _repositorio.Buscar(null, null, IdMensaje);
                if (mensajeEncontrado == null)
                    throw new TaskCanceledException("El mensaje no Existe");

                bool respuesta = await _repositorio.Eliminar(mensajeEncontrado.IdMensaje);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> IdDestinatario(string correo)
        {
            int idCliente = await _repositorioCliente.ObtenerIdCorreoCliente(correo);
            int idUsuario = await _repositorioUsuario.ObtenerIdCorreoUsuario(correo);
            if (idUsuario == 0 && idCliente == 0)
            {
                throw new TaskCanceledException("El Correo no Existe");
            }
            else if (idUsuario != 0)
            {
                idCliente = idUsuario;
            }
            return idCliente;

        }



        public async Task<int> IdRemitente(string correo)
        {
            int idCliente = await _repositorioCliente.ObtenerIdCorreoCliente(correo);
            int idUsuario = await _repositorioUsuario.ObtenerIdCorreoUsuario(correo);
            if(idUsuario==0 && idCliente==0)
            {
               throw new TaskCanceledException("El Correo no Existe");
            }            
            else if (idUsuario != 0){
                  idCliente=idUsuario;
            }
            return idCliente;
        }

        public async Task<List<Mensajes>> Lista()
        {
            List<Mensajes> query=await _repositorio.Lista();
            return query;
        }

        public async Task<string> NombreDelDestinatario(string tipo,int IdDestinatario)
        {
            string nombreCompleto = "";
            if (tipo == "Cliente")
            {
                nombreCompleto = await _repositorioCliente.ObtenerNombreCompleto(IdDestinatario);
                
            }
            else if (tipo == "Usuario")
            {
                nombreCompleto = await _repositorioUsuario.ObtenerNombreCompleto(IdDestinatario);
            }

            return nombreCompleto;         
        }

        public async Task<string> NombreDelRemitente(string tipo,int IdRemitente)
        {
            string nombreCompleto = "";
            if (tipo == "Cliente")
            {
                nombreCompleto = await _repositorioCliente.ObtenerNombreCompleto(IdRemitente);

            }
            else if (tipo == "Usuario")
            {
                nombreCompleto = await _repositorioUsuario.ObtenerNombreCompleto(IdRemitente);
            }

            return nombreCompleto;
        }

        public async Task<Mensajes> Registrar(Mensajes entidad,DestinatarioMensaje destino)
        {
            Mensajes mensajeExiste = await _repositorio.Buscar(null,entidad.Asunto,null);

            if (mensajeExiste != null)
                throw new TaskCanceledException("El mensaje ya existe");

            try
            {
                Mensajes mensajeCreado = await _repositorio.Crear(entidad,destino);
                if (mensajeCreado.IdMensaje == 0)
                    throw new TaskCanceledException("No se Puedo crear el Mensaje");
                return mensajeCreado;

            }
            catch (Exception)
            {

                throw;
            }
                

        }

        public async Task<Mensajes> RegistrarMensaje(Mensajes entidad, DestinatarioMensaje destino)
        {
            //Mensajes mensajeExiste = await _repositorio.Buscar(null, entidad.Asunto, null);

            //if (mensajeExiste != null)
            //    throw new TaskCanceledException("El mensaje ya existe");

            try
            {
                Mensajes mensajeCreado = await _repositorio.CrearRespuestaMensaje(entidad, destino);
                if (mensajeCreado.IdMensaje == 0)
                    throw new TaskCanceledException("No se Puedo crear el Mensaje");
                return mensajeCreado;

            }
            catch (Exception)
            {

                throw;
            }


        }

        public Task<string> NombreUserDestinatario(int IdMensaje)
        {
            throw new NotImplementedException();
        }

        public async Task<string> correoDestinatario(string Tipo, int IdDestinatario)
        {
            string nombreCorreo = "";
            if (Tipo == "Cliente")
            {
                nombreCorreo = await _repositorioCliente.ObtenerCorreo(IdDestinatario);

            }
            else if (Tipo == "Usuario")
            {
                nombreCorreo = await _repositorioUsuario.ObtenerCorreo(IdDestinatario);
            }

            return nombreCorreo;
        }

        public async Task<string> correoRemitente(string Tipo, int IdRemitente)
        {
            string nombreCorreo = "";
            if (Tipo == "Cliente")
            {
                nombreCorreo = await _repositorioCliente.ObtenerCorreo(IdRemitente);

            }
            else if (Tipo == "Usuario")
            {
                nombreCorreo = await _repositorioUsuario.ObtenerCorreo(IdRemitente);
                                                      }

            return nombreCorreo;
        }

        public async Task<List<Mensajes>> BuscarFechas(string searchTerm,DateTime? fechaBusquedaUtc, string busqueda = "")
        {

            var ListaDeMensajes = await _repositorio.Lista();
            var clientelista = await _repositorioCliente.Lista();
            List<Mensajes> mensajes = new List<Mensajes>();
            var ListaDestinatarioMensaje = await _destinatarioMensajeService.Lista();

            var idCliente = clientelista.Where(x => x.Dni == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
            var IndiceDestinatario = ListaDestinatarioMensaje.Where(x => x.IdDestinatario == idCliente).Select(x => x.IdMensaje).ToList();
            var mensajesRemitentes = ListaDeMensajes.Where(p => p.IdRemitente == idCliente && p.IdRespuestaMensaje == null).ToList();

            if (IndiceDestinatario.Count > 0)
            {
                var mensajesDestinatario = ListaDeMensajes.Where(p => IndiceDestinatario.Contains(p.IdMensaje)).ToList();
                mensajes.AddRange(mensajesDestinatario);
            }

            if (mensajesRemitentes.Count > 0)
            {
                mensajes.AddRange(mensajesRemitentes);
            }

            var MisMensajes = mensajes.Where(p =>string.IsNullOrWhiteSpace(busqueda) 
                || p.Asunto.ToLower().Contains(busqueda.ToLower()) || (
                fechaBusquedaUtc.HasValue && p.FechaDeMensaje.HasValue
                && p.FechaDeMensaje.Value >= fechaBusquedaUtc.Value.Date
                && p.FechaDeMensaje.Value < fechaBusquedaUtc.Value.Date.AddDays(1)));


            return MisMensajes.ToList();


        }
    }
}
