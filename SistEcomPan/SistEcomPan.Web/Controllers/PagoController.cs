using Entidades;
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
        private readonly ICategoriaService _categoriaService;
        private readonly IDetallePagoService _detallePagoService;

        public PagoController(IPagoService pagoService, IClienteService clienteService, IPedidoService pedidoService, ICategoriaService categoriaService, IDetallePagoService detallePagoService)
        {
            _pagoService = pagoService;
            _clienteService = clienteService;
            _pedidoService = pedidoService;
            _categoriaService = categoriaService;
            _detallePagoService = detallePagoService;
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
            var Pagolista = await _pagoService.Lista();
            List<VMPago> vmPagolista = new List<VMPago>();
            var pedidoCliente = await _pedidoService.ObtenerNombre();
            var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in Pagolista)
            {
                var clientePedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().IdCliente;          
                var nombreCliente = clientes.Where(x => x.IdCliente == clientePedido).First().Nombres + "" +
                                        clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;

                var codigoPedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().Codigo;
                var fechaPedido = pedidoCliente.Where(x => x.IdPedido == item.IdPedido).First().FechaPedido;
                vmPagolista.Add(new VMPago
                {
                    IdPago = item.IdPago,
                    IdPedido= item.IdPedido,
                    MontoDePedido = Convert.ToString(item.MontoDePedido),
                    Descuento = Convert.ToString(item.Descuento),
                    MontoTotalDePago =Convert.ToString(item.MontoTotalDePago),
                    MontoDeuda = Convert.ToString(item.MontoDeuda),
                    FechaPago = Convert.ToDateTime(item.FechaDePago),
                    Estado = item.Estado,
                    NombreCliente=nombreCliente,
                    CodigoPedido=codigoPedido,
                    FechaPedido=fechaPedido
                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmPagolista });
        }



        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] VMPago modelo)
        {
            GenericResponse<VMPago> gResponse = new GenericResponse<VMPago>();

            try
            {
                List<Pagos> listaPagos = new List<Pagos>();         
                if (modelo != null)
                {
                    listaPagos.Add(new Pagos
                    {
                        IdPedido=modelo.IdPedido,
                        MontoDePedido=Convert.ToDecimal(modelo.MontoDePedido),
                        Descuento=Convert.ToDecimal(modelo.Descuento),
                        MontoTotalDePago=Convert.ToDecimal(modelo.MontoTotalDePago),
                        MontoDeuda=Convert.ToDecimal(modelo.MontoDeuda),
                        Estado = modelo.Estado,
                        DetallePago = modelo.DetallePago.Select(detalle => new DetallePago
                        {
                            MontoAPagar = Convert.ToDecimal(detalle.MontoAPagar),
                            PagoDelCliente=Convert.ToDecimal(detalle.PagoDelCliente),
                            DeudaDelCliente= Convert.ToDecimal(detalle.DeudaDelCliente),
                            CambioDelCliente=Convert.ToDecimal(detalle.CambioDelCliente)
                        }).ToList()

                    });
                    
                }

                Pagos pagoCreado = await _pagoService.Registrar(listaPagos.First());

                List<VMPago> vmPagolista = new List<VMPago>();
                
                if (pagoCreado != null)
                {

                    vmPagolista.Add(new VMPago
                    {
                        IdPago=pagoCreado.IdPago,
                        IdPedido = pagoCreado.IdPedido,
                        MontoDePedido = Convert.ToString(pagoCreado.MontoDePedido),
                        Descuento = Convert.ToString(pagoCreado.Descuento),
                        MontoTotalDePago = Convert.ToString(pagoCreado.MontoTotalDePago),
                        MontoDeuda =Convert.ToString(pagoCreado.MontoDeuda),
                        Estado = pagoCreado.Estado,
                        DetallePago = pagoCreado.DetallePago.Select(detalle => new VMDetallePago
                        {
                            MontoAPagar = Convert.ToString(detalle.MontoAPagar),
                            PagoDelCliente = Convert.ToString(detalle.PagoDelCliente),
                            DeudaDelCliente = Convert.ToString(detalle.DeudaDelCliente),
                            CambioDelCliente = Convert.ToString(detalle.CambioDelCliente)

                        }).ToList()

                    });
                   
                }

                gResponse.Estado = true;
                gResponse.objeto = vmPagolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMPago modelo)
        {
            GenericResponse<VMPago> gResponse = new GenericResponse<VMPago>();

            try
            {
                List<Pagos> listaPagos = new List<Pagos>();
                if (modelo != null)
                {
                    listaPagos.Add(new Pagos
                    {
                        IdPago=modelo.IdPago,
                        IdPedido = modelo.IdPedido,
                        MontoDeuda = Convert.ToDecimal(modelo.MontoDeuda),
                        Estado = modelo.Estado,
                        DetallePago = modelo.DetallePago.Select(detalle => new DetallePago
                        {
                            MontoAPagar = Convert.ToDecimal(detalle.MontoAPagar),
                            PagoDelCliente = Convert.ToDecimal(detalle.PagoDelCliente),
                            DeudaDelCliente = Convert.ToDecimal(detalle.DeudaDelCliente),
                            CambioDelCliente = Convert.ToDecimal(detalle.CambioDelCliente)
                        }).ToList()

                    });

                }

                Pagos pagoCreado = await _pagoService.Editar(listaPagos.First());

                List<VMPago> vmPagolista = new List<VMPago>();

                if (pagoCreado != null)
                {

                    vmPagolista.Add(new VMPago
                    {
                        IdPago = pagoCreado.IdPago,
                        IdPedido = pagoCreado.IdPedido,
                        MontoDeuda = Convert.ToString(pagoCreado.MontoDeuda),
                        Estado = pagoCreado.Estado,
                        NombreCliente=modelo.NombreCliente,
                        FechaPago= Convert.ToDateTime(pagoCreado.FechaDePago),
                        MontoTotalDePago=modelo.MontoTotalDePago,
                        MontoDePedido=modelo.MontoDePedido,
                        Descuento=modelo.Descuento,
                        DetallePago = pagoCreado.DetallePago.Select(detalle => new VMDetallePago
                        {
                            MontoAPagar = Convert.ToString(detalle.MontoAPagar),
                            PagoDelCliente = Convert.ToString(detalle.PagoDelCliente),
                            DeudaDelCliente = Convert.ToString(detalle.DeudaDelCliente),
                            CambioDelCliente = Convert.ToString(detalle.CambioDelCliente)

                        }).ToList()

                    });

                }

                gResponse.Estado = true;
                gResponse.objeto = vmPagolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPagoPedido(int searchTerm)
        {
            var Pagolista = await _pagoService.Lista();
            var clientelista = await _clienteService.Lista();
            var PedidosLista = await _pedidoService.Lista();

            //var idCliente = clientelista.Where(x => x.id == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
            //var estadoCliente = Pedidolista.Where(x => x.IdCliente == idCliente).Select(x => x.Estado).FirstOrDefault();
            var pagosFiltrados = Pagolista.Where(x => x.IdPedido == searchTerm).ToList();

            //var PagosPedidos = PedidosLista.Where(x => x.IdPedido == idPedido[i])

            // Filtro de búsqueda por término de búsqueda (searchTerm)



            var MisPagos = pagosFiltrados;

            List<VMPago> vmPagos = new List<VMPago>();

            foreach (var item in MisPagos)
            {
                vmPagos.Add(new VMPago
                {
                    IdPago = item.IdPago,
                    MontoDePedido = Convert.ToString(item.MontoDePedido),
                    Descuento = Convert.ToString(item.Descuento),
                    CodigoPedido = PedidosLista.Where(x => x.IdPedido == item.IdPedido).Select(x => x.Codigo).First(),
                    MontoTotalDePago = Convert.ToString(item.MontoTotalDePago),
                    MontoDeuda = Convert.ToString(item.MontoDeuda),
                    FechaPago = item.FechaDePago,
                    Estado = item.Estado
                });
            }


            // Paginación
            var pagosPaginados = vmPagos.ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = pagosPaginados, totalItems = vmPagos.Count() });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMisPagos(string searchTerm, string busqueda = "")
        {
            var Pagolista = await _pagoService.Lista();
            var clientelista = await _clienteService.Lista();
            var PedidosLista = await _pedidoService.Lista();

            var idCliente = clientelista.Where(x => x.Dni == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
            //var estadoCliente = Pedidolista.Where(x => x.IdCliente == idCliente).Select(x => x.Estado).FirstOrDefault();
            var idPedido = PedidosLista.Where(x => x.IdCliente == idCliente).Select(x => x.IdPedido).ToList();

            //var PagosPedidos = PedidosLista.Where(x => x.IdPedido == idPedido[i])

            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pagolista.Where(p => idPedido.Contains(p.IdPedido)).ToList();
            

            var MisPagos = pedidosFiltrados.Where(p =>
            string.IsNullOrWhiteSpace(busqueda) || p.Estado.ToLower().Contains(busqueda.ToLower()) ||
            p.FechaDePago.Date == (DateTime.TryParse(busqueda, out DateTime fechaBusqueda) ? fechaBusqueda.Date : p.FechaDePago.Date)
            );

            List<VMPago> vmPagos = new List<VMPago>();

            foreach (var item in MisPagos)
            {
                vmPagos.Add(new VMPago
                {
                    IdPago = item.IdPago,
                    MontoDePedido=Convert.ToString(item.MontoDePedido),  
                    Descuento=Convert.ToString(item.Descuento),
                    CodigoPedido = PedidosLista.Where(x => x.IdPedido == item.IdPedido).Select(x => x.Codigo).First(),
                    MontoTotalDePago = Convert.ToString(item.MontoTotalDePago),
                    MontoDeuda=Convert.ToString(item.MontoDeuda), 
                    FechaPago=item.FechaDePago,
                    Estado = item.Estado
                }); 
            }


            // Paginación
            var pagosPaginados = vmPagos.ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = pagosPaginados, totalItems = vmPagos.Count()});
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMiDetallePago(int idPago, int page = 1, int itemsPerPage = 4)
        {
            var DetallePagolista = await _detallePagoService.Lista();
            var Pedidolista = await _pedidoService.Lista();

            var MisPedidos = DetallePagolista.Where(p => p.IdPago.Equals(idPago)
            );

         
            List<VMDetallePago> vmDetallePagos = new List<VMDetallePago>();

            foreach (var item in MisPedidos)
            {
                vmDetallePagos.Add(new VMDetallePago
                {
                    IdDetallePago = item.IdDetallePago,
                    IdPago = item.IdPago,
                    MontoAPagar=Convert.ToString(item.MontoAPagar),
                    PagoDelCliente=Convert.ToString(item.PagoDelCliente),
                    DeudaDelCliente=Convert.ToString(item.DeudaDelCliente),
                    CambioDelCliente=Convert.ToString(item.CambioDelCliente)
                });
            }


            // Paginación
            var pagosPaginados = vmDetallePagos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { detallePago = pagosPaginados, totalItems = vmDetallePagos.Count() });
        }

    }

}

