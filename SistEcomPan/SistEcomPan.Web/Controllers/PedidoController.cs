
using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using Negocio.Implementacion;
using SistEcomPan.Web.Models.ViewModels;
using Newtonsoft.Json;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IDetallePedidoService _detallePedidoService;
        
        public PedidoController(IPedidoService pedidoService,IClienteService clienteService, IProductoService productoService,ICategoriaService categoriaService,IDetallePedidoService detallePedidoService )
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _productoService = productoService;
            _categoriaService = categoriaService;
            _detallePedidoService = detallePedidoService;
        }
        public IActionResult NuevoPedido()
        {

            return View();
        }

        public IActionResult HistorialPedido()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _pedidoService.Lista();
            List<VMPedido> vmListaPedidos = new List<VMPedido>();
            var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in lista)
            {
                vmListaPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    NombresCompletos= clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +
                    clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos,
                    FechaPedido=item.FechaPedido
                 
                }) ;
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaPedidos });
        }

        [HttpGet]
        public async Task<IActionResult> ListaNombres()
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in Clientelista)
            {
                vmClientelista.Add(new VMCliente
                {
                    IdCliente=item.IdCliente,
                    NombreCompleto = clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +" "+
                    clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos

                });
            }
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string searchTerm = "", int page = 1, int itemsPerPage = 5)
       {
            var Productolista = await _productoService.Lista();

            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var productosFiltrados = Productolista.Where(p =>
                string.IsNullOrWhiteSpace(searchTerm) || p.Descripcion.ToLower().Contains(searchTerm.ToLower())
            ) ;

            var categoriaProducto = productosFiltrados.First().IdCategoria;
            var categorias = await _categoriaService.ObtenerNombre();
            var categoriaEncontrada = categorias.Where(x => x.IdCategoria == categoriaProducto).First().TipoDeCategoria;
            // Paginación
            var productosPaginados = productosFiltrados.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK,new { productos = productosPaginados, totalItems = productosFiltrados.Count() ,categoria=categoriaEncontrada});
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPedidos(string searchTerm = "", int page = 1, int itemsPerPage = 5)
        {
            var Pedidolista = await _pedidoService.Lista();

            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pedidolista.Where(p =>
                string.IsNullOrWhiteSpace(searchTerm) || p.Codigo.ToLower().Contains(searchTerm.ToLower())
            );

            var clientePedido = pedidosFiltrados.First().IdCliente;
            var clientes = await _clienteService.ObtenerNombre();
            var clienteEncontrado = clientes.Where(x => x.IdCliente ==clientePedido).First().Nombres +""+  
                                    clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;
            // Paginación
            var pedidosPaginados = pedidosFiltrados.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = pedidosFiltrados.Count(), nombreCliente = clienteEncontrado });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePedido(string searchTerm = "", int page = 1, int itemsPerPage = 5)
        {
            var Pedidolista = await _pedidoService.Lista();
            var DetallePedidoLista = await _detallePedidoService.Lista();
            var Productos = await _productoService.Lista();
          
            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pedidolista.Where(p =>
                string.IsNullOrWhiteSpace(searchTerm) || p.Codigo.ToLower().Contains(searchTerm.ToLower()
                )
            );

            var codigos = pedidosFiltrados.First().IdPedido;

            var detallePedidosFiltrados = DetallePedidoLista.Where(p => p.IdPedido == codigos).ToList();
           

            var clientePedido = pedidosFiltrados.First().IdCliente;
            var clientes = await _clienteService.ObtenerNombre();
            var clienteEncontrado = clientes.Where(x => x.IdCliente == clientePedido).First().Nombres + "" +
                                    clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;

            var productoPedido = detallePedidosFiltrados.Where(x=>x.IdPedido==codigos).Select(x=>x.IdProducto).ToArray();
            var productos = await _productoService.ObtenerNombre();

            List<string> nombresProductos = new List<string>();
            

            foreach (var item in productoPedido)
            {
                var productoEncontrado = productos.Where(x => x.IdProducto == item).Select(x => x.Descripcion).FirstOrDefault();
                if (productoEncontrado!=null) {
                    nombresProductos.Add(productoEncontrado); 
                }
            }                      
            // Paginación
            var pedidosPaginados = detallePedidosFiltrados.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = detallePedidosFiltrados.Count(),codigo=codigos, nombreCliente = clienteEncontrado,nombresProducto=nombresProductos});
        }

        [HttpGet]
        public async Task<IActionResult> ListaClientes(string numeroDocumento)
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var clientes = await _clienteService.ObtenerNombre();
            var nombreCompletos = clientes.Where(x => x.Dni == numeroDocumento).First().Nombres + " " +
            clientes.Where(x => x.Dni == numeroDocumento).First().Apellidos;
            var idcliente = clientes.Where(x => x.Dni == numeroDocumento).First().IdCliente;

            vmClientelista.Add(new VMCliente
            {
               IdCliente =idcliente,
               NombreCompleto=nombreCompletos
            });
            
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }

        [HttpGet]
        public async Task<IActionResult> ListaNumeroDocumento(string nombreCompleto)
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var clientes = await _clienteService.ObtenerNombre();           
            var nombres = clientes.FirstOrDefault(x => nombreCompleto.StartsWith(x.Nombres)).Nombres;
            var apellidos = clientes.FirstOrDefault(x => nombreCompleto.EndsWith(x.Apellidos)).Apellidos;
             vmClientelista.Add(new VMCliente
             {
                 IdCliente = clientes.Where(x => x.Apellidos == apellidos && x.Nombres == nombres).First().IdCliente,
                 Dni = clientes.Where(x => x.Apellidos==apellidos && x.Nombres==nombres).First().Dni
             }) ;
            
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }



        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMPedido modelo)
        {
            GenericResponse<VMPedido> gResponse = new GenericResponse<VMPedido>();

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
                            FechaPedido = pedidoCreado.FechaPedido,
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