using Datos.Interfaces;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Negocio.Implementacion;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class DevolucionController : Controller
    {
        private readonly IDevolucionService _devolucionService;
        private readonly IDetalleDevolucionService _DetalleDevolucionService;
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        public DevolucionController(IDevolucionService devolucionService, IPedidoService pedidoService, IClienteService clienteService, IDetalleDevolucionService detalleDevolucionService)
        {
            _devolucionService = devolucionService;
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _DetalleDevolucionService = detalleDevolucionService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMDevolucion modelo)
        {
            GenericResponse<VMDevolucion> gResponse = new GenericResponse<VMDevolucion>();

            try
            {
                List<Devolucion> listaDevolucion = new List<Devolucion>();
                List<VMPedido> listaVMPedidos = new List<VMPedido>();
                //var clientes = await _devolucionService.ObtenerNombre();
                if (modelo != null)
                {
                    listaDevolucion.Add(new Devolucion
                    {
                        CodigoPedido = modelo.CodigoPedido,
                        MontoPedido = modelo.MontoPedido,
                        Descuento = modelo.Descuento,
                        MontoAPagar = modelo.MontoAPagar,
                        DetalleDevolucion = modelo.DetalleDevolucion.Select(detalle => new DetalleDevolucion
                        {
                            Categoria = detalle.Categoria,
                            Descripcion=detalle.Descripcion,
                            Precio= Convert.ToDecimal(detalle.Precio),
                            CantidadPedido = detalle.CantidadPedido,
                            Total = Convert.ToDecimal(detalle.Total),
                            CantidadDevolucion = detalle.CantidadDevolucion
                        }).ToList()


                    });

                }

                Devolucion DevolucionCreada = await _devolucionService.Registrar(listaDevolucion.First());

                List<VMDevolucion> vmPedidolista = new List<VMDevolucion>();
                List<Pedidos> listPedidos = new List<Pedidos>();
                if (DevolucionCreada != null)
                {
                    //listPedidos.Add(pedidoCreado);
                    //foreach (var item in listPedidos)
                    //{
                    vmPedidolista.Add(new VMDevolucion
                    {
                        CodigoPedido= DevolucionCreada.CodigoPedido,
                        CodigoDevolucion= DevolucionCreada.CodigoDevolucion,
                        MontoPedido = DevolucionCreada.MontoPedido,
                        Descuento= DevolucionCreada.Descuento,
                        MontoAPagar = DevolucionCreada.MontoAPagar
                    });
                    //}
                }

                gResponse.Estado = true;
                gResponse.objeto = vmPedidolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpGet]
        public async Task<IActionResult> ListaDevoluciones()
        {
            var DevolucionLista = await _devolucionService.Lista();
       
            List<VMDevolucion> vmDevoluciones = new List<VMDevolucion>();

            foreach (var item in DevolucionLista)
            {
                var IdCliente = await _pedidoService.ObtenerIdCliente(item.CodigoPedido);
                vmDevoluciones.Add(new VMDevolucion
                {
                    IdDevolucion = item.IdDevolucion,
                    CodigoPedido = item.CodigoPedido,
                    CodigoDevolucion = item.CodigoDevolucion,
                    MontoPedido = item.MontoPedido,
                    Descuento = item.Descuento,
                    MontoAPagar = item.MontoAPagar,
                    FechaDevolucion= item.FechaDevolucion,
                    //NombresCompletos= clientes.Where(x => x.IdCliente ==item.IdCliente).FirstOrDefault().Nombres + "" +
                    //                  clientes.Where(x => x.IdCliente == item.IdCliente).FirstOrDefault().Apellidos
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(IdCliente)

                });
            }

            return StatusCode(StatusCodes.Status200OK, new { data = vmDevoluciones });




        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleDevolucion(int idDevolucion, int page = 1, int itemsPerPage = 4)
        {
            var DetalleDevolucionlista = await _DetalleDevolucionService.Lista();
            var DevolucionCliente = DetalleDevolucionlista.Where(p => p.IdDevolucion.Equals(idDevolucion)
            );

            //var Productos = await _productoService.ObtenerNombre();
            List<VMDetalleDevolucion> vmDetalleDevolucion = new List<VMDetalleDevolucion>();

            foreach (var item in DevolucionCliente)
            {
                vmDetalleDevolucion.Add(new VMDetalleDevolucion
                {
                    IdDetalleDevolucion = item.IdDetalleDevolucion,
                    Categoria = item.Categoria,
                    Precio = Convert.ToString(item.Precio),
                    Descripcion = item.Descripcion,
                    CantidadPedido=item.CantidadPedido,
                    CantidadDevolucion = item.CantidadDevolucion,
                    Total = Convert.ToString(item.Total)
                });
            }


            // Paginación
            var DetalleDevolucionPaginados = vmDetalleDevolucion.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { detalleDevolucion = DetalleDevolucionPaginados, totalItems = vmDetalleDevolucion.Count() });
        }
    }
}
