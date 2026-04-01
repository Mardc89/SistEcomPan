
using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using Negocio.Implementacion;
using SistEcomPan.Web.Models.ViewModels;
using Newtonsoft.Json;
using SistEcomPan.Web.Tools.Response;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System;

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IDetallePedidoService _detallePedidoService;
        private readonly ITimeZoneService _timeZoneService;


        public PedidoController(IPedidoService pedidoService,IClienteService clienteService, IProductoService productoService,ICategoriaService categoriaService,IDetallePedidoService detallePedidoService,ITimeZoneService timeZoneService )
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _productoService = productoService;
            _categoriaService = categoriaService;
            _detallePedidoService = detallePedidoService;
            _timeZoneService = timeZoneService;
        }

     
        public IActionResult NuevoPedido()
        {

            return View();
        }

        public IActionResult Index()
        {

            return View();
        }


        public IActionResult ListaPedidos()
        {

            return View();
        }

        public IActionResult HistorialPedido()
        {

            return View();
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult MisPedidos()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string codigo, string fechaInicio, string fechaFin)
        {
            List<Pedidos> HistorialPedido = await _pedidoService.Historial(codigo, fechaInicio, fechaFin);

            List <VMPedido> vmListaPedidos = new List<VMPedido>();

            //var timeZoneId = Request.Headers["X-TimeZone"].ToString();
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            //var tz = TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZoneId)? "UTC" : timeZoneId);

            //TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);


            foreach (var item in HistorialPedido)
            {
                vmListaPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Dni=await _clienteService.ObtenerDni(item.IdCliente),
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(item.IdCliente),
                    FechaPedido = item.FechaPedido.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaPedido.Value, userTimeZone) : null,

                });
            }

            return StatusCode(StatusCodes.Status200OK, vmListaPedidos);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _pedidoService.Lista();
            List<VMPedido> vmListaPedidos = new List<VMPedido>();

            //var timeZoneId = Request.Headers["X-TimeZone"].ToString();

            //var tz = TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZoneId)? "UTC" : timeZoneId);
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            foreach (var item in lista)
            {
                vmListaPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(item.IdCliente),
                    FechaPedido = item.FechaPedido.HasValue?TimeZoneInfo.ConvertTimeFromUtc(item.FechaPedido.Value,userTimeZone):null

                }) ;
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaPedidos });
        }

        [HttpGet]
        public async Task<IActionResult> ListaNombres()
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            foreach (var item in Clientelista)
            {
                vmClientelista.Add(new VMCliente
                {
                    IdCliente=item.IdCliente,
                    NombreCompleto = await _clienteService.ObtenerNombreCompleto(item.IdCliente)

                });
            }
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string searchTerm = "", int page = 1, int itemsPerPage = 3)
        {
            var resultado = await _productoService.ObtenerProductos(searchTerm, page, itemsPerPage);

            return StatusCode(StatusCodes.Status200OK, new
            {
                productos = resultado.listaProductos,
                totalItems = resultado.totalItems
            });
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerPedidos(string searchTerm = "", int page = 1, int itemsPerPage = 4)
        {
            var resultado = await _pedidoService.ObtenerPedidos(searchTerm, page, itemsPerPage);

            return Ok(new
            {
                pedidos = resultado.Item1,
                totalItems = resultado.totalItems
            });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerListaPedidos()
        {
            var Pedidolista = await _pedidoService.Lista();
            List<VMPedido> vmPedidos = new List<VMPedido>();
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
            foreach (var item in Pedidolista)
            {
                vmPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    FechaDeEntrega = item.FechaDeEntrega.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaDeEntrega.Value, userTimeZone) : null,
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(item.IdCliente)

                });
            }

            return StatusCode(StatusCodes.Status200OK, new { data=vmPedidos});




        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAllDetallePedido(int idPedido)
        {
            var DetallePedidolista = await _detallePedidoService.Lista();
            var Pedidolista = await _pedidoService.Lista();

            var MisPedidos = DetallePedidolista.Where(p => p.IdPedido.Equals(idPedido)
            );

            var Productos = await _productoService.ObtenerNombre();
            List<VMDetallePedido> vmDetallePedidos = new List<VMDetallePedido>();

            foreach (var item in MisPedidos)
            {
                vmDetallePedidos.Add(new VMDetallePedido
                {
                    IdDetallePedido = item.IdDetallePedido,
                    IdPedido = item.IdPedido,
                    IdProducto=item.IdProducto,
                    DescripcionProducto = Productos.Where(x => x.IdProducto == item.IdProducto).First().Descripcion,
                    Cantidad = item.Cantidad,
                    Precio = Convert.ToString(Productos.Where(x => x.IdProducto == item.IdProducto).First().Precio),
                    Total = Convert.ToString(item.Total)
                });
            }


            // Paginación
            //var pedidosPaginados = vmDetallePedidos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { detallePedido =vmDetallePedidos , totalItems = vmDetallePedidos.Count() });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMisPedidos(string searchTerm,string busqueda="")
        {

            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);
            DateTime? fechaBusquedaUtc = _timeZoneService.ConvertirFecha(busqueda, userTimeZone);

            //var MisPedidos = pedidosFiltrados.Where(p =>
            //string.IsNullOrWhiteSpace(busqueda) || p.Estado.ToLower().Contains(busqueda.ToLower()) 
            //||
            //p.FechaPedido.Value.Date==(DateTime.TryParse(busqueda,out DateTime fechaBusqueda)?fechaBusqueda.Date:p.FechaPedido.Value.Date)
            //);

            var MisPedidos = await _pedidoService.MisPedidos(searchTerm,fechaBusquedaUtc,busqueda);


            List<VMPedido> vmPedidos = new List<VMPedido>();

            foreach (var item in MisPedidos)
            {
                vmPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    FechaPedido = item.FechaPedido.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.FechaPedido.Value, userTimeZone) : null,

                });
            }

            //var clientes = await _clienteService.ObtenerNombre();
            var clienteEncontrado = await _pedidoService.NombreDelCliente(searchTerm);
            // Paginación
            var pedidosPaginados = vmPedidos.ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = pedidosPaginados, totalItems = vmPedidos.Count(), nombreCliente = clienteEncontrado });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerMiDetallePedido(int idPedido, int page = 1, int itemsPerPage = 4)
        {
            var DetallePedidolista = await _detallePedidoService.Lista();
            var Pedidolista = await _pedidoService.Lista();

            var MisPedidos = DetallePedidolista.Where(p => p.IdPedido.Equals(idPedido)
            );

            var Productos = await _productoService.ObtenerNombre();
            List<VMDetallePedido> vmDetallePedidos = new List<VMDetallePedido>();

            foreach (var item in MisPedidos)
            {
                vmDetallePedidos.Add(new VMDetallePedido
                {
                    IdDetallePedido = item.IdDetallePedido,
                    IdPedido = item.IdPedido,
                    DescripcionProducto = Productos.Where(x=>x.IdProducto==item.IdProducto).First().Descripcion,
                    Cantidad = item.Cantidad,
                    Precio = Convert.ToString(Productos.Where(x=>x.IdProducto==item.IdProducto).First().Precio),
                    Total  = Convert.ToString(item.Total)
                });
            }


            // Paginación
            var pedidosPaginados = vmDetallePedidos.Skip((page - 1)*itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { detallePedido = pedidosPaginados, totalItems = vmDetallePedidos.Count()});
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleFinal(string searchTerm, int page = 1, int itemsPerPage = 3)
        {

            var (items, total) =
                await _pedidoService.ObtenerDetalleFinal(searchTerm);

            var pedidos = items
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .Select(x => new VMDetallePedido
                {
                    DescripcionProducto = x.DescripcionProducto,
                    CategoriaProducto = _categoriaService
                        .ObtenerNombre()
                        .Result
                        .First(c => c.IdCategoria == x.IdCategoria)
                        .TipoDeCategoria,
                    Precio = x.Precio.ToString(),
                    Cantidad = x.CantidadTotal,
                    Total = x.Total.ToString()
                })
                .ToList();

            return Ok(new
            {
                pedidos,
                totalItems = total,
                codigos = searchTerm
            });

            //// Paginación
            //var pedidosPaginados = vmDetallePedido.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            //return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = vmDetallePedido.Count(), codigos = codigo });
        }


        [HttpGet]
        public async Task<IActionResult> ListaClientes(string numeroDocumento)
   {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var cliente = await _clienteService.ObtenerPorDni(numeroDocumento);

            vmClientelista.Add(new VMCliente
            {
                IdCliente = cliente.IdCliente,
                NombreCompleto = cliente.Apellidos +" "+cliente.Nombres,
                Direccion = cliente.Direccion,
                Telefono = cliente.Telefono

            });

            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }

        [HttpGet]
        public async Task<IActionResult> ListaNumeroDocumento(string nombreCompleto)
        {
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var cliente = await _clienteService.BuscarCliente(nombreCompleto);

            vmClientelista.Add(new VMCliente
            {
                IdCliente = cliente.IdCliente,
                Dni = cliente.Dni,
                Direccion = cliente.Direccion,
                Telefono = cliente.Telefono
            });


            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMPedido modelo)
        {
            GenericResponse<VMPedido> gResponse = new GenericResponse<VMPedido>();
            //var timeZoneId = Request.Headers["X-TimeZone"].ToString();
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            try
            {
                List<Pedidos> listaPedidos = new List<Pedidos>();
                List<VMPedido> listaVMPedidos = new List<VMPedido>();
                List<DetallePedido> detallePedido = new List<DetallePedido>();
                var clientes = await _clienteService.ObtenerNombre();
                if (modelo != null)
                {
                    //listaVMPedidos.Add(modelo);
                    //foreach (var item in listaVMPedidos)
                    //{
                        listaPedidos.Add(new Pedidos
                        {
                            IdCliente = clientes.Where(x=>x.Dni==modelo.Dni).First().IdCliente,
                            //MontoTotal = Convert.ToDecimal(item.MontoTotal),
                            Estado = modelo.Estado,
                            FechaDeEntrega = modelo.FechaDeEntrega?.ToUniversalTime(),
                            DetallePedido = modelo.DetallePedido.Select(detalle => new DetallePedido
                            { 
                              IdProducto=detalle.IdProducto,
                              Cantidad=detalle.Cantidad,
                              Total=Convert.ToDecimal(detalle.Total)
                            }).ToList()

                        }); 
                    //}
                }              

                Pedidos pedidoCreado = await _pedidoService.Registrar(listaPedidos.First());

                //var tz = TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZoneId) 
                //        ? "UTC" : timeZoneId);

                List<VMPedido> vmPedidolista = new List<VMPedido>();
                List<Pedidos> listPedidos = new List<Pedidos>();
                if (pedidoCreado != null)
                {
                    //listPedidos.Add(pedidoCreado);
                    //foreach (var item in listPedidos)
                    //{
                        vmPedidolista.Add(new VMPedido
                        {
                            IdPedido = pedidoCreado.IdPedido,
                            IdCliente = pedidoCreado.IdCliente,
                            Codigo = pedidoCreado.Codigo,
                            MontoTotal = Convert.ToString(pedidoCreado.MontoTotal),
                            FechaPedido = pedidoCreado.FechaPedido.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(pedidoCreado.FechaPedido.Value, userTimeZone) : null,
                            Estado = pedidoCreado.Estado,
                            DetallePedido = pedidoCreado.DetallePedido.Select(detalle => new VMDetallePedido
                            {
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                Total = Convert.ToString(detalle.Total)
                            }).ToList()
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

        [HttpPost]
        public async Task<IActionResult> ActualizarPedido([FromBody] VMPedido modelo)
        {
            GenericResponse<VMPedido> gResponse = new GenericResponse<VMPedido>();
            //var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            TimeZoneInfo userTimeZone = _timeZoneService.GetTimeZone(Request);

            try
            {
                List<Pedidos> listaPedidos = new List<Pedidos>();
                List<VMPedido> listaVMPedidos = new List<VMPedido>();
                List<DetallePedido> detallePedido = new List<DetallePedido>();
            
                if (modelo != null)
                {
                    //listaVMPedidos.Add(modelo);
                    //foreach (var item in listaVMPedidos)
                    //{
                    listaPedidos.Add(new Pedidos
                    {
                        IdPedido=modelo.IdPedido,
                        //IdCliente = clientes.Where(x => x.Dni == modelo.Dni).First().IdCliente,
                        MontoTotal = Convert.ToDecimal(modelo.MontoTotal),
                   
                        DetallePedido = modelo.DetallePedido.Select(detalle => new DetallePedido
                        {
                            IdPedido=detalle.IdPedido,
                            IdProducto = detalle.IdProducto,
                            Cantidad = detalle.Cantidad,
                            Total = Convert.ToDecimal(detalle.Total)
                        }).ToList()

                    });
                    //}
                }

                Pedidos pedidoCreado = await _pedidoService.Actualizar(listaPedidos.First());

                List<VMPedido> vmPedidolista = new List<VMPedido>();
                List<Pedidos> listPedidos = new List<Pedidos>();
                if (pedidoCreado != null)
                {
                    //listPedidos.Add(pedidoCreado);
                    //foreach (var item in listPedidos)
                    //{
                    vmPedidolista.Add(new VMPedido
                    {
                        IdPedido = modelo.IdPedido,
                        MontoTotal = Convert.ToString(pedidoCreado.MontoTotal),
                        Codigo=modelo.Codigo,
                        NombresCompletos=modelo.NombresCompletos,
                        FechaPedido = pedidoCreado.FechaPedido.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(pedidoCreado.FechaPedido.Value, userTimeZone) : null,
                        Estado = modelo.Estado,
                        DetallePedido = pedidoCreado.DetallePedido.Select(detalle => new VMDetallePedido
                        {
                            IdPedido=detalle.IdPedido,
                            IdProducto = detalle.IdProducto,
                            Cantidad = detalle.Cantidad,
                            Total = Convert.ToString(detalle.Total)
                        }).ToList()
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



        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdPedido)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _pedidoService.Eliminar(IdPedido);

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