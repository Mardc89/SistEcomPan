using Entidades;
using Microsoft.AspNetCore.Mvc;
using Negocio.Implementacion;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoService _pagoService;
        private readonly IClienteService _clienteService;
        private readonly IPedidoService _pedidoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IDetallePedidoService _detallePedidoService;

        public PagoController(IPagoService pagoService, IClienteService clienteService, IPedidoService pedidoService, ICategoriaService categoriaService, IDetallePedidoService detallePedidoService)
        {
            _pagoService = pagoService;
            _clienteService = clienteService;
            _pedidoService = pedidoService;
            _categoriaService = categoriaService;
            _detallePedidoService = detallePedidoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NuevoPago()
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
                        MontoDePedido = Convert.ToDecimal(modelo.MontoDePedido),
                        MontoTotalDePago = Convert.ToDecimal(modelo.MontoTotalDePago),
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
                        MontoDePedido = Convert.ToString(pagoCreado.MontoDePedido),
                        MontoTotalDePago = Convert.ToString(pagoCreado.MontoTotalDePago),
                        MontoDeuda = Convert.ToString(pagoCreado.MontoDeuda),
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

    }

}

