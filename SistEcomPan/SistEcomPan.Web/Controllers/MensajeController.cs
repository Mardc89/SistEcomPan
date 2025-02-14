﻿using Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Implementacion;
using Negocio.Interfaces;
using NuGet.ContentModel;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Handler;
using SistEcomPan.Web.Tools.Response;
using System.Text.Json;

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class MensajeController : Controller
    {
        private readonly IMensajeService _mensajeService;
        private readonly IClienteService _clienteService;
        private readonly IDestinatarioMensajeService _destinatarioMensajeService;
        public MensajeController(IMensajeService mensajeService, IClienteService clienteService, IDestinatarioMensajeService destinatarioMensajeService)
        {
            _mensajeService = mensajeService;
            _clienteService = clienteService;
            _destinatarioMensajeService = destinatarioMensajeService;
        }
        public IActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult MisMensajes()
        {

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMisMensajes(string searchTerm, string busqueda = "")
        {
            var ListaDeMensajes = await _mensajeService.Lista();
            var clientelista = await _clienteService.Lista();
            List<Mensajes> mensajes = new List<Mensajes>();
            var ListaDestinatarioMensaje = await _destinatarioMensajeService.Lista();
            var idCliente = clientelista.Where(x => x.Dni == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
            var IndiceDestinatario = ListaDestinatarioMensaje.Where(x => x.IdDestinatario == idCliente).Select(x => x.IdMensaje).ToList();
            var mensajesRemitentes = ListaDeMensajes.Where(p => p.IdRemitente == idCliente  && p.IdRespuestaMensaje == null).ToList();
            if (IndiceDestinatario.Count>0)
            {
                var mensajesDestinatario = ListaDeMensajes.Where(p =>IndiceDestinatario.Contains(p.IdMensaje)).ToList();
                mensajes.AddRange(mensajesDestinatario);
            }

            if (mensajesRemitentes.Count>0)
            {
                mensajes.AddRange(mensajesRemitentes);
            }
            

            var MisMensajes = mensajes.Where(p =>
            string.IsNullOrWhiteSpace(busqueda) || p.Asunto.ToLower().Contains(busqueda.ToLower()) ||
            p.FechaDeMensaje.Date == (DateTime.TryParse(busqueda, out DateTime fechaBusqueda) ? fechaBusqueda.Date : p.FechaDeMensaje.Date)
            );

            List<VMMensaje> vmMensajes = new List<VMMensaje>();

            foreach (var item in MisMensajes)
            {
                vmMensajes.Add(new VMMensaje
                {
                    IdMensaje = item.IdMensaje,
                    Cuerpo = Convert.ToString(item.Cuerpo),
                    IdRespuestaMensaje = item.IdRespuestaMensaje,
                    Asunto = Convert.ToString(item.Asunto),
                    NombreDestinatario = await _destinatarioMensajeService.NombreDelDestinatario(item.IdMensaje),
                    NombreRemitente = await _mensajeService.NombreDelRemitente(item.Remitente, item.IdRemitente),
                    FechaDeMensaje = item.FechaDeMensaje,
                    CorreoRemitente = await _mensajeService.correoRemitente(item.Remitente, item.IdRemitente),
                    CorreoDestinatario = await _destinatarioMensajeService.correoDestinatario(item.IdMensaje)
                });
            }


            // Paginación
            var misMensajesPaginados = vmMensajes.ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = misMensajesPaginados, totalItems = vmMensajes.Count() });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMensajeDeAsunto(string asunto)
        {
            string asuntoAbuscar=asunto.Contains(":")?asunto.Substring(asunto.IndexOf(":") + 1).Trim():asunto;
            
            var ListaDeMensajes = await _mensajeService.Lista();
            var MisMensajes = ListaDeMensajes.Where(p => p.Asunto == asuntoAbuscar || p.Asunto.Contains(asuntoAbuscar)).ToList();

            List<VMMensaje> vmMensajes = new List<VMMensaje>();

            foreach (var item in MisMensajes)
            {
                vmMensajes.Add(new VMMensaje
                {
                    IdMensaje = item.IdMensaje,
                    Cuerpo = Convert.ToString(item.Cuerpo),
                    NombreDestinatario = await _destinatarioMensajeService.NombreDelDestinatario(item.IdMensaje),
                    NombreRemitente = await _mensajeService.NombreDelRemitente(item.Remitente, item.IdRemitente),
                    FechaDeMensaje=item.FechaDeMensaje

                });
            }

            var mensajeAsunto = vmMensajes.ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = mensajeAsunto, totalItems = vmMensajes.Count() });
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerMiDetalleMensaje(string asunto, int page = 1, int itemsPerPage = 4)
        {
            var DetalleMensajelista = await _mensajeService.Lista();
            var MisMensajes = DetalleMensajelista.Where(p =>p.Asunto.Contains(asunto)&& p.IdRespuestaMensaje!=null).ToList();

            List<VMMensaje> vmDetalleMensajes = new List<VMMensaje>();

            foreach (var item in MisMensajes)
            {
                vmDetalleMensajes.Add(new VMMensaje
                {
                    IdMensaje = item.IdMensaje,
                    Remitente = Convert.ToString(item.Remitente),
                    Cuerpo = Convert.ToString(item.Cuerpo),
                    IdRespuestaMensaje = item.IdRespuestaMensaje,
                    Asunto = Convert.ToString(item.Asunto),
                    FechaDeMensaje = item.FechaDeMensaje

                });
            }


            // Paginación
            var mensajesPaginados = vmDetalleMensajes.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { detalleMensaje = mensajesPaginados, totalItems = vmDetalleMensajes.Count() });
        }


        [HttpGet]
        public async Task<IActionResult> ListaMensajes()
        {
            var lista = await _mensajeService.Lista();
            List<VMMensaje> vmListaMensajes = new List<VMMensaje>();
            foreach (var item in lista)
            {
                vmListaMensajes.Add(new VMMensaje
                {
                    IdMensaje = Convert.ToInt32(item.IdMensaje),
                    Asunto = item.Asunto,
                    Cuerpo = item.Cuerpo,
                    NombreDestinatario = await _destinatarioMensajeService.NombreDelDestinatario(item.IdMensaje),
                    NombreRemitente = await _mensajeService.NombreDelRemitente(item.Remitente,item.IdRemitente),
                    FechaDeMensaje = item.FechaDeMensaje,
                    CorreoRemitente=await _mensajeService.correoRemitente(item.Remitente,item.IdRemitente),
                    CorreoDestinatario=await _destinatarioMensajeService.correoDestinatario(item.IdMensaje)

                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaMensajes });
        }


        [HttpPost]
        public async Task<IActionResult> EnviarMensajeRespuesta([FromBody] VMRemitenteDestinatario modelo)
        {
            GenericResponse<VMMensaje> gResponse = new GenericResponse<VMMensaje>();

            try
            {
                List<Mensajes> listaMensajes = new List<Mensajes>();
                List<VMRemitenteDestinatario> listaVMMensajes = new List<VMRemitenteDestinatario>();
                List<DestinatarioMensaje> listaDestinoMensajes = new List<DestinatarioMensaje>();
                List<VMDestinatarioMensaje> listaDestinoVMMensajes = new List<VMDestinatarioMensaje>();
                if (modelo != null)
                {
                    listaVMMensajes.Add(modelo);
                    foreach (var item in listaVMMensajes)
                    {
                        listaMensajes.Add(new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(item.RemitenteMensaje.IdMensaje),
                            IdRemitente= await _mensajeService.IdRemitente(item.RemitenteMensaje.CorreoRemitente),
                            Asunto = item.RemitenteMensaje.Asunto,
                            Cuerpo = item.RemitenteMensaje.Cuerpo,
                            Remitente=await _mensajeService.Remitente(item.RemitenteMensaje.CorreoRemitente),
                            IdRespuestaMensaje=item.RemitenteMensaje.IdRespuestaMensaje
                        });
                    }

                    foreach (var item in listaVMMensajes)
                    {
                        listaDestinoMensajes.Add(new DestinatarioMensaje
                        {
                            IdMensaje = Convert.ToInt32(item.DestinatarioMensaje.IdMensaje),
                            IdDestinatario =await _mensajeService.IdDestinatario(item.DestinatarioMensaje.CorreoDestinatario),
                            Destinatario =await _mensajeService.Destinatario(item.DestinatarioMensaje.CorreoDestinatario)
                        });
                    }
                }

                Mensajes mensajeCreado = await _mensajeService.RegistrarMensaje(listaMensajes.First(),listaDestinoMensajes.First());

                List<VMMensaje> vmMensajelista = new List<VMMensaje>();
                List<Mensajes> listMensajes = new List<Mensajes>();
                if (mensajeCreado != null)
                {
                    listMensajes.Add(mensajeCreado);


                    foreach (var item in listMensajes)
                    {
                        vmMensajelista.Add(new VMMensaje
                        {
                            IdMensaje = Convert.ToInt32(item.IdMensaje),
                            Asunto = item.Asunto,
                            Cuerpo = item.Cuerpo,
                            NombreDestinatario = await _destinatarioMensajeService.NombreDelDestinatario(item.IdMensaje),
                            NombreRemitente = await _mensajeService.NombreDelRemitente(item.Remitente, item.IdRemitente),
                            FechaDeMensaje= item.FechaDeMensaje

                        }); 
                    }
                }

                string mensajeJson = JsonSerializer.Serialize(vmMensajelista);
                await MensajeWebSocketHandler.SendMessageAsync(mensajeJson);

                gResponse.Estado = true;
                gResponse.objeto = vmMensajelista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMRemitenteDestinatario modelo)
        {
            GenericResponse<VMMensaje> gResponse = new GenericResponse<VMMensaje>();



            try
            {
                List<Mensajes> listaMensajes = new List<Mensajes>();
                List<VMRemitenteDestinatario> listaVMMensajes = new List<VMRemitenteDestinatario>();
                List<DestinatarioMensaje> listaDestinoMensajes = new List<DestinatarioMensaje>();
                List<VMDestinatarioMensaje> listaDestinoVMMensajes = new List<VMDestinatarioMensaje>();
                if (modelo != null)
                {
                    listaVMMensajes.Add(modelo);
                    foreach (var item in listaVMMensajes)
                    {
                        listaMensajes.Add(new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(modelo.RemitenteMensaje.IdMensaje),
                            IdRemitente = await _mensajeService.IdRemitente(modelo.RemitenteMensaje.CorreoRemitente),
                            Asunto = modelo.RemitenteMensaje.Asunto,
                            Cuerpo = modelo.RemitenteMensaje.Cuerpo,
                            Remitente = await _mensajeService.Remitente(modelo.RemitenteMensaje.CorreoRemitente),
                            IdRespuestaMensaje = modelo.RemitenteMensaje.IdRespuestaMensaje
                        });
                    }

                    foreach (var item in listaVMMensajes)
                    {
                        listaDestinoMensajes.Add(new DestinatarioMensaje
                        {
                            IdMensaje = Convert.ToInt32(modelo.DestinatarioMensaje.IdMensaje),
                            IdDestinatario = await _mensajeService.IdDestinatario(modelo.DestinatarioMensaje.CorreoDestinatario),
                            Destinatario = await _mensajeService.Destinatario(modelo.DestinatarioMensaje.CorreoDestinatario)
                        });

                    }
                }
                                     
               
                Mensajes mensajeCreado = await _mensajeService.Registrar(listaMensajes.First(), listaDestinoMensajes.First());

                List<VMMensaje> vmMensajelista = new List<VMMensaje>();
                List<Mensajes> listMensajes = new List<Mensajes>();
                if (mensajeCreado != null)
                {
                    listMensajes.Add(mensajeCreado);
                    foreach (var item in listMensajes)
                    {                  

                    vmMensajelista.Add(new VMMensaje
                    {
                        IdMensaje = Convert.ToInt32(mensajeCreado.IdMensaje),
                        Asunto = mensajeCreado.Asunto,
                        Cuerpo = mensajeCreado.Cuerpo,
                        NombreDestinatario = await _destinatarioMensajeService.NombreDelDestinatario(mensajeCreado.IdMensaje),
                        NombreRemitente = await _mensajeService.NombreDelRemitente(mensajeCreado.Remitente, mensajeCreado.IdRemitente),
                        FechaDeMensaje = mensajeCreado.FechaDeMensaje

                    });  


                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmMensajelista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMRemitenteDestinatario modelo)
        {
            GenericResponse<VMMensaje> gResponse = new GenericResponse<VMMensaje>();

            try
            {

                List<Mensajes> listaMensajes = new List<Mensajes>();
                List<VMRemitenteDestinatario> listaVMMensajes = new List<VMRemitenteDestinatario>();
                //List<VMMensaje> listaVMMensajes = new List<VMMensaje>();
                if (modelo != null)
                {
                    listaVMMensajes.Add(modelo);
                    foreach (var item in listaVMMensajes)
                    {
                        listaMensajes.Add(new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(item.RemitenteMensaje.IdMensaje),
                            IdRemitente = Convert.ToInt32(item.RemitenteMensaje.IdRemitente),
                            Asunto = item.RemitenteMensaje.Asunto,
                            Cuerpo = item.RemitenteMensaje.Cuerpo
                        });
                    }
                }

                Mensajes mensajeEditado = await _mensajeService.Editar(listaMensajes.First());

                List<Mensajes> listMensajes = new List<Mensajes>();
                List<VMMensaje> vmMensajelista = new List<VMMensaje>();
                if (mensajeEditado != null)
                {
                    listMensajes.Add(mensajeEditado);
                    foreach (var item in listMensajes)
                    {
                        vmMensajelista.Add(new VMMensaje
                        {
                            IdMensaje = Convert.ToInt32(item.IdMensaje),
                            IdRemitente = Convert.ToInt32(item.IdRemitente),
                            Asunto = item.Asunto,
                            Cuerpo = item.Cuerpo
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmMensajelista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdMensaje)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _mensajeService.Eliminar(IdMensaje);

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
    }
}
