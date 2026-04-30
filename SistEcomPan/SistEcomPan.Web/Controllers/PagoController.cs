using Entidades;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Implementacion;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IPagoService _pagoService;
        private readonly IClienteService _clienteService;
        private readonly IPedidoService _pedidoService;
        private readonly IDetallePagoService _detallePagoService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IMapper _mapper;
        public PagoController(IPagoService pagoService, IClienteService clienteService, IPedidoService pedidoService, IDetallePagoService detallePagoService, ITimeZoneService timeZoneService, IMapper mapper)
        {
            _pagoService = pagoService;
            _clienteService = clienteService;
            _pedidoService = pedidoService;
            _detallePagoService = detallePagoService;
            _timeZoneService = timeZoneService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NuevoPago()
        {
            return View();
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult MisPagos()
        {

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            var pagoLista = await _pagoService.Lista();
            var pedidoLista = await _pedidoService.Lista();
            var clienteLista = await _clienteService.Lista();

            // Diccionarios para búsqueda rápida
            var pedidosDict = pedidoLista.ToDictionary(x => x.IdPedido);
            var clientesDict = clienteLista.ToDictionary(x => x.IdCliente);

            List<VMPago> vmPagoLista = new();

            foreach (var item in pagoLista)
            {
                // Entity -> ViewModel con Mapster
                VMPago vmPago = _mapper.Map<VMPago>(item);

                if (pedidosDict.TryGetValue(item.IdPedido, out var pedido))
                {
                    vmPago.CodigoPedido = pedido.Codigo;
                    vmPago.FechaPedido = pedido.FechaPedido;

                    if (clientesDict.TryGetValue(pedido.IdCliente, out var cliente))
                    {
                        vmPago.NombreCliente = $"{cliente.Nombres} {cliente.Apellidos}";
                    }
                }

                vmPago.FechaPago = item.FechaDePago.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(
                        item.FechaDePago.Value,
                        userTimeZone)
                    : null;

                vmPagoLista.Add(vmPago);
            }

            return Ok(new { data = vmPagoLista });
        }

        //[HttpGet]
        //public async Task<IActionResult> Lista()
        //{
        //    TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
        //    var Pagolista = await _pagoService.Lista();
        //    List<VMPago> vmPagolista = new List<VMPago>();
        //    var pedidoCliente = await _pedidoService.Lista();
        //    var clientes = await _clienteService.Lista();
        //    foreach (var item in Pagolista)
        //    {
        //        var clientePedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().IdCliente;          
        //        var nombreCliente = clientes.Where(x => x.IdCliente == clientePedido).First().Nombres + "" +
        //                                clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;

        //        var codigoPedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().Codigo;
        //        var fechaPedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().FechaPedido;
        //        vmPagolista.Add(new VMPago
        //        {
        //            IdPago = item.IdPago,
        //            IdPedido= item.IdPedido,
        //            MontoDePedido = Convert.ToString(item.MontoDePedido),
        //            Descuento = Convert.ToString(item.Descuento),
        //            MontoTotalDePago =Convert.ToString(item.MontoTotalDePago),
        //            MontoDeuda = Convert.ToString(item.MontoDeuda),
        //            //FechaPago = Convert.ToDateTime(item.FechaDePago),
        //            FechaPago = item.FechaDePago.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaDePago.Value, userTimeZone) : null,
        //            Estado = item.Estado,
        //            NombreCliente=nombreCliente,
        //            CodigoPedido=codigoPedido,
        //            FechaPedido=fechaPedido
        //        });
        //    }
        //    return StatusCode(StatusCodes.Status200OK, new { data = vmPagolista });
        //}



        //[HttpPost]
        //public async Task<IActionResult> Guardar([FromBody] VMPago modelo)
        //{
        //    GenericResponse<VMPago> gResponse = new GenericResponse<VMPago>();
        //    TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
        //    try
        //    {
        //        List<Pagos> listaPagos = new List<Pagos>();         
        //        if (modelo != null)
        //        {
        //            listaPagos.Add(new Pagos
        //            {
        //                IdPedido=modelo.IdPedido,
        //                MontoDePedido=Convert.ToDecimal(modelo.MontoDePedido),
        //                Descuento=Convert.ToDecimal(modelo.Descuento),
        //                MontoTotalDePago=Convert.ToDecimal(modelo.MontoTotalDePago),
        //                MontoDeuda=Convert.ToDecimal(modelo.MontoDeuda),
        //                Estado = modelo.Estado,
        //                DetallePago = modelo.DetallePago.Select(detalle => new DetallePago
        //                {
        //                    MontoAPagar = Convert.ToDecimal(detalle.MontoAPagar),
        //                    PagoDelCliente=Convert.ToDecimal(detalle.PagoDelCliente),
        //                    DeudaDelCliente= Convert.ToDecimal(detalle.DeudaDelCliente),
        //                    CambioDelCliente=Convert.ToDecimal(detalle.CambioDelCliente)
        //                }).ToList()

        //            });

        //        }

        //        Pagos pagoCreado = await _pagoService.Registrar(listaPagos.First());

        //        List<VMPago> vmPagolista = new List<VMPago>();

        //        if (pagoCreado != null)
        //        {

        //            vmPagolista.Add(new VMPago
        //            {
        //                IdPago=pagoCreado.IdPago,
        //                IdPedido = pagoCreado.IdPedido,
        //                MontoDePedido = Convert.ToString(pagoCreado.MontoDePedido),
        //                Descuento = Convert.ToString(pagoCreado.Descuento),
        //                MontoTotalDePago = Convert.ToString(pagoCreado.MontoTotalDePago),
        //                MontoDeuda =Convert.ToString(pagoCreado.MontoDeuda),
        //                Estado = pagoCreado.Estado,
        //                FechaPago = pagoCreado.FechaDePago.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(pagoCreado.FechaDePago.Value, userTimeZone) : null,
        //                DetallePago = pagoCreado.DetallePago.Select(detalle => new VMDetallePago
        //                {
        //                    MontoAPagar = Convert.ToString(detalle.MontoAPagar),
        //                    PagoDelCliente = Convert.ToString(detalle.PagoDelCliente),
        //                    DeudaDelCliente = Convert.ToString(detalle.DeudaDelCliente),
        //                    CambioDelCliente = Convert.ToString(detalle.CambioDelCliente)


        //                }).ToList()

        //            });

        //        }

        //        gResponse.Estado = true;
        //        gResponse.objeto = vmPagolista.First();

        //    }
        //    catch (Exception ex)
        //    {
        //        gResponse.Estado = false;
        //        gResponse.Mensaje = ex.Message;

        //    }

        //    return StatusCode(StatusCodes.Status200OK, gResponse);

        //}

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] VMPago modelo)
        {
            GenericResponse<VMPago> gResponse = new();
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            try
            {
                // ViewModel -> Entity
                Pagos pago = _mapper.Map<Pagos>(modelo);

                // Guardar
                Pagos pagoCreado = await _pagoService.Registrar(pago);

                if (pagoCreado == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se pudo registrar el pago.";
                    return Ok(gResponse);
                }

                // Entity -> ViewModel
                VMPago vmPago = _mapper.Map<VMPago>(pagoCreado);

                // Convertir zona horaria
                vmPago.FechaPago = pagoCreado.FechaDePago.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(
                        pagoCreado.FechaDePago.Value,
                        userTimeZone)
                    : null;

                gResponse.Estado = true;
                gResponse.objeto = vmPago;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return Ok(gResponse);
        }


        //[HttpPut]
        //public async Task<IActionResult> Editar([FromBody] VMPago modelo)
        //{
        //    GenericResponse<VMPago> gResponse = new GenericResponse<VMPago>();
        //    TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
        //    try
        //    {
        //        List<Pagos> listaPagos = new List<Pagos>();
        //        if (modelo != null)
        //        {
        //            listaPagos.Add(new Pagos
        //            {
        //                IdPago=modelo.IdPago,
        //                IdPedido = modelo.IdPedido,
        //                MontoDeuda = Convert.ToDecimal(modelo.MontoDeuda),
        //                Estado = modelo.Estado,
        //                DetallePago = modelo.DetallePago.Select(detalle => new DetallePago
        //                {
        //                    MontoAPagar = Convert.ToDecimal(detalle.MontoAPagar),
        //                    PagoDelCliente = Convert.ToDecimal(detalle.PagoDelCliente),
        //                    DeudaDelCliente = Convert.ToDecimal(detalle.DeudaDelCliente),
        //                    CambioDelCliente = Convert.ToDecimal(detalle.CambioDelCliente)
        //                }).ToList()

        //            });

        //        }

        //        Pagos pagoCreado = await _pagoService.Editar(listaPagos.First());

        //        List<VMPago> vmPagolista = new List<VMPago>();

        //        if (pagoCreado != null)
        //        {
        //            var pedidoCliente = await _pedidoService.ObtenerNombre();
        //            var fechaPedido = pedidoCliente.Where(x => x.IdPedido == modelo.IdPedido).First().FechaPedido;
        //            vmPagolista.Add(new VMPago
        //            {
        //                IdPago = pagoCreado.IdPago,
        //                IdPedido = pagoCreado.IdPedido,
        //                MontoDeuda = Convert.ToString(pagoCreado.MontoDeuda),
        //                Estado = pagoCreado.Estado,
        //                NombreCliente=modelo.NombreCliente,
        //                MontoTotalDePago=modelo.MontoTotalDePago,
        //                MontoDePedido=modelo.MontoDePedido,
        //                Descuento=modelo.Descuento,                           
        //                //FechaPago= Convert.ToDateTime(pagoCreado.FechaDePago),
        //                FechaPago = pagoCreado.FechaDePago.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(pagoCreado.FechaDePago.Value, userTimeZone) : null,
        //                FechaPedido =fechaPedido,
        //                DetallePago = pagoCreado.DetallePago.Select(detalle => new VMDetallePago
        //                {
        //                    MontoAPagar = Convert.ToString(detalle.MontoAPagar),
        //                    PagoDelCliente = Convert.ToString(detalle.PagoDelCliente),
        //                    DeudaDelCliente = Convert.ToString(detalle.DeudaDelCliente),
        //                    CambioDelCliente = Convert.ToString(detalle.CambioDelCliente),


        //                }).ToList()

        //            });

        //        }

        //        gResponse.Estado = true;
        //        gResponse.objeto = vmPagolista.First();

        //    }
        //    catch (Exception ex)
        //    {
        //        gResponse.Estado = false;
        //        gResponse.Mensaje = ex.Message;

        //    }

        //    return StatusCode(StatusCodes.Status200OK, gResponse);

        //}


        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMPago modelo)
        {
            GenericResponse<VMPago> gResponse = new();
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            try
            {
                // Convertir ViewModel -> Entity
                Pagos pago = _mapper.Map<Pagos>(modelo);

                // Editar
                Pagos pagoEditado = await _pagoService.Editar(pago);

                if (pagoEditado == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se pudo editar el pago.";
                    return Ok(gResponse);
                }

                // Obtener datos adicionales
                var pedidoCliente = await _pedidoService.ObtenerNombre();

                var fechaPedido = pedidoCliente
                    .FirstOrDefault(x => x.IdPedido == modelo.IdPedido)?
                    .FechaPedido;

                // Convertir Entity -> ViewModel
                VMPago vmPago = pagoEditado.Adapt<VMPago>();

                // Propiedades adicionales que no vienen del entity
                vmPago.NombreCliente = modelo.NombreCliente;
                vmPago.MontoTotalDePago = modelo.MontoTotalDePago;
                vmPago.MontoDePedido = modelo.MontoDePedido;
                vmPago.Descuento = modelo.Descuento;
                vmPago.FechaPedido = fechaPedido;

                vmPago.FechaPago = pagoEditado.FechaDePago.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(
                        pagoEditado.FechaDePago.Value,
                        userTimeZone)
                    : null;

                gResponse.Estado = true;
                gResponse.objeto = vmPago;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return Ok(gResponse);
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerPagoPedido(int searchTerm)
        {
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            var pagoLista = await _pagoService.Lista();
            var pedidoLista = await _pedidoService.Lista();

            // Filtrar pagos por pedido
            var pagosFiltrados = pagoLista
                .Where(x => x.IdPedido == searchTerm)
                .ToList();

            // Diccionario para búsquedas rápidas
            var pedidosDict = pedidoLista.ToDictionary(x => x.IdPedido);

            List<VMPago> vmPagos = new();

            foreach (var item in pagosFiltrados)
            {
                // Entity -> ViewModel
                VMPago vmPago = _mapper.Map<VMPago>(item);

                if (pedidosDict.TryGetValue(item.IdPedido, out var pedido))
                {
                    vmPago.CodigoPedido = pedido.Codigo;
                }

                vmPago.FechaPago = item.FechaDePago.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(
                        item.FechaDePago.Value,
                        userTimeZone)
                    : null;

                vmPagos.Add(vmPago);
            }

            return Ok(new
            {
                data = vmPagos,
                totalItems = vmPagos.Count
            });
        }


        //[HttpGet]
        //public async Task<IActionResult> ObtenerPagoPedido(int searchTerm)
        //{
        //    var Pagolista = await _pagoService.Lista();
        //    var clientelista = await _clienteService.Lista();
        //    var PedidosLista = await _pedidoService.Lista();
        //    var pagosFiltrados = Pagolista.Where(x => x.IdPedido == searchTerm).ToList();
        //    var MisPagos = pagosFiltrados;

        //    TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

        //    List<VMPago> vmPagos = new List<VMPago>();

        //    foreach (var item in MisPagos)
        //    {
        //        vmPagos.Add(new VMPago
        //        {
        //            IdPago = item.IdPago,
        //            MontoDePedido = Convert.ToString(item.MontoDePedido),
        //            Descuento = Convert.ToString(item.Descuento),
        //            CodigoPedido = PedidosLista.Where(x => x.IdPedido == item.IdPedido).Select(x => x.Codigo).First(),
        //            MontoTotalDePago = Convert.ToString(item.MontoTotalDePago),
        //            MontoDeuda = Convert.ToString(item.MontoDeuda),
        //            FechaPago = item.FechaDePago.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaDePago.Value, userTimeZone) : null,
        //            Estado = item.Estado
        //        });
        //    }


        //    // Paginación
        //    var pagosPaginados = vmPagos.ToList();

        //    return StatusCode(StatusCodes.Status200OK, new { data = pagosPaginados, totalItems = vmPagos.Count() });
        //}


        //[HttpGet]
        //public async Task<IActionResult> ObtenerMisPagos(string searchTerm, string busqueda = "")
        //{
        //    var Pagolista = await _pagoService.Lista();
        //    var clientelista = await _clienteService.Lista();
        //    var PedidosLista = await _pedidoService.Lista();

        //    TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
        //    DateTime? fechaBusquedaUtc = _timeZoneService.ConvertirFecha(busqueda, userTimeZone);

        //    var idCliente = clientelista.Where(x => x.Dni == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
        //    var idPedido = PedidosLista.Where(x => x.IdCliente == idCliente).Select(x => x.IdPedido).ToList();
        //    var pedidosFiltrados = Pagolista.Where(p => idPedido.Contains(p.IdPedido)).ToList();


        //    var MisPagos = pedidosFiltrados.Where(p =>
        //    string.IsNullOrWhiteSpace(busqueda) || p.Estado.ToLower().Contains(busqueda.ToLower())
        //    );

        //    List<VMPago> vmPagos = new List<VMPago>();

        //    foreach (var item in MisPagos)
        //    {
        //        vmPagos.Add(new VMPago
        //        {
        //            IdPago = item.IdPago,
        //            MontoDePedido=Convert.ToString(item.MontoDePedido),  
        //            Descuento=Convert.ToString(item.Descuento),
        //            CodigoPedido = PedidosLista.Where(x => x.IdPedido == item.IdPedido).Select(x => x.Codigo).First(),
        //            MontoTotalDePago = Convert.ToString(item.MontoTotalDePago),
        //            MontoDeuda=Convert.ToString(item.MontoDeuda), 
        //            FechaPago = item.FechaDePago.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaDePago.Value, userTimeZone) : null,
        //            Estado = item.Estado
        //        }); 
        //    }

        //    var pagosPaginados = vmPagos.ToList();

        //    return StatusCode(StatusCodes.Status200OK, new { data = pagosPaginados, totalItems = vmPagos.Count()});
        //}

        [HttpGet]
        public async Task<IActionResult> ObtenerMisPagos(string searchTerm, string busqueda = "")
        {
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            var pagoLista = await _pagoService.Lista();
            var clienteLista = await _clienteService.Lista();
            var pedidoLista = await _pedidoService.Lista();

            // Buscar cliente por DNI
            var idCliente = clienteLista
                .FirstOrDefault(x => x.Dni == searchTerm)?
                .IdCliente;

            if (idCliente == null)
            {
                return Ok(new
                {
                    data = new List<VMPago>(),
                    totalItems = 0
                });
            }

            // Obtener pedidos del cliente
            var idsPedido = pedidoLista
                .Where(x => x.IdCliente == idCliente)
                .Select(x => x.IdPedido)
                .ToHashSet();

            // Filtrar pagos
            var misPagos = pagoLista
                .Where(p => idsPedido.Contains(p.IdPedido))
                .Where(p =>
                    string.IsNullOrWhiteSpace(busqueda) ||
                    p.Estado.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Diccionario pedidos
            var pedidosDict = pedidoLista.ToDictionary(x => x.IdPedido);

            List<VMPago> vmPagos = new();

            foreach (var item in misPagos)
            {
                VMPago vmPago = item.Adapt<VMPago>();

                if (pedidosDict.TryGetValue(item.IdPedido, out var pedido))
                {
                    vmPago.CodigoPedido = pedido.Codigo;
                }

                vmPago.FechaPago = item.FechaDePago.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(
                        item.FechaDePago.Value,
                        userTimeZone)
                    : null;

                vmPagos.Add(vmPago);
            }

            return Ok(new
            {
                data = vmPagos,
                totalItems = vmPagos.Count
            });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMiDetallePago(int idPago,int page = 1,int itemsPerPage = 4)
        {
            var detallePagoLista = await _detallePagoService.Lista();

            // Filtrar detalle por IdPago
            var misPagos = detallePagoLista
                .Where(x => x.IdPago == idPago)
                .ToList();

            // Convertir Entity -> ViewModel con Mapster
            List<VMDetallePago> vmDetallePagos = _mapper.Map<List<VMDetallePago>>(misPagos);

            // Paginación
            var pagosPaginados = vmDetallePagos
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return Ok(new
            {
                detallePago = pagosPaginados,
                totalItems = vmDetallePagos.Count
            });
        }


        //[HttpGet]
        //public async Task<IActionResult> ObtenerMiDetallePago(int idPago, int page = 1, int itemsPerPage = 4)
        //{
        //    var DetallePagolista = await _detallePagoService.Lista();
        //    var Pedidolista = await _pedidoService.Lista();

        //    var MisPedidos = DetallePagolista.Where(p => p.IdPago.Equals(idPago)
        //    );


        //    List<VMDetallePago> vmDetallePagos = new List<VMDetallePago>();

        //    foreach (var item in MisPedidos)
        //    {
        //        vmDetallePagos.Add(new VMDetallePago
        //        {
        //            IdDetallePago = item.IdDetallePago,
        //            IdPago = item.IdPago,
        //            MontoAPagar=Convert.ToString(item.MontoAPagar),
        //            PagoDelCliente=Convert.ToString(item.PagoDelCliente),
        //            DeudaDelCliente=Convert.ToString(item.DeudaDelCliente),
        //            CambioDelCliente=Convert.ToString(item.CambioDelCliente)
        //        });
        //    }


        //    // Paginación
        //    var pagosPaginados = vmDetallePagos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

        //    return StatusCode(StatusCodes.Status200OK, new { detallePago = pagosPaginados, totalItems = vmDetallePagos.Count() });
        //}

    }

}

