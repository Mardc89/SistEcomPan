
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
                    FechaPedido = item.FechaPedido

                });
            }

            return StatusCode(StatusCodes.Status200OK, vmListaPedidos);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _pedidoService.Lista();
            List<VMPedido> vmListaPedidos = new List<VMPedido>();
            //var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in lista)
            {
                vmListaPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    //NombresCompletos= clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +
                    //clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos,
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(item.IdCliente),
                    FechaPedido =item.FechaPedido
                 
                }) ;
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaPedidos });
        }

        [HttpGet]
        public async Task<IActionResult> ListaNombres()
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            //var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in Clientelista)
            {
                vmClientelista.Add(new VMCliente
                {
                    IdCliente=item.IdCliente,
                    //NombreCompleto = clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +" "+
                    //clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos
                    NombreCompleto = await _clienteService.ObtenerNombreCompleto(item.IdCliente)

                });
            }
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string searchTerm = "", int page = 1, int itemsPerPage = 3)
       {

            var Productolista = await _productoService.Lista();

            List<string> categoriaEncontrada = new List<string>();


            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var productosFiltrados = Productolista.Where(p =>
                string.IsNullOrWhiteSpace(searchTerm) || p.Descripcion.ToLower().Contains(searchTerm.ToLower())
            ) ;

            var categoriaProducto = productosFiltrados.Select(x => x.IdCategoria).ToArray();
            var categorias = await _categoriaService.ObtenerNombre();
            
            for (int i=0;i<categoriaProducto.Count();i++) {
                var categoriasDeProductos=categorias.Where(x => x.IdCategoria == categoriaProducto[i]).First().TipoDeCategoria;
                categoriaEncontrada.Add(categoriasDeProductos);
            }
            // Paginación
            var productosPaginados = productosFiltrados.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK,new { productos = productosPaginados, totalItems = productosFiltrados.Count(), categoria = categoriaEncontrada});
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPedidos(string searchTerm = "", int page = 1, int itemsPerPage = 4)
        {
            var Pedidolista = await _pedidoService.Lista();
            var clientes = await _clienteService.Lista();  
            var pedidosPendientes = Pedidolista.Where(p => p.Estado.Equals("Nuevo")||p.Estado.Equals("Existe Deuda")).ToList();
            int?IDdeCliente=null;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                IDdeCliente = clientes

                .Where(p => p.Apellidos.ToLower().Contains(searchTerm.ToLower()) || p.Nombres.ToLower().Contains(searchTerm.ToLower()))
                .Select(p => (int?)p.IdCliente)
                .FirstOrDefault();
            }
          
            // Filtro de búsqueda por término de búsqueda (searchTerm)
          

            //var pedidosFiltrados = pedidosPendientes.Where(p => p.IdCliente==1).ToList();


            var pedidosFiltrados = pedidosPendientes.Where(p =>
                string.IsNullOrWhiteSpace(searchTerm) || p.Codigo.Contains(searchTerm.ToLower()) ||(IDdeCliente!=null && p.IdCliente.Equals(IDdeCliente))
            );

            List<VMPedido> vmPedidos = new List<VMPedido>();

            foreach (var item in pedidosFiltrados)
            {
                vmPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    FechaPedido = item.FechaPedido,
                    //NombresCompletos= clientes.Where(x => x.IdCliente ==item.IdCliente).FirstOrDefault().Nombres + "" +
                    //                  clientes.Where(x => x.IdCliente == item.IdCliente).FirstOrDefault().Apellidos
                    NombresCompletos = await _clienteService.ObtenerNombreCompleto(item.IdCliente)

                });
            }


            // Paginación
            var pedidosPaginados = vmPedidos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = vmPedidos.Count() });




        }

        [HttpGet]
        public async Task<IActionResult> ObtenerListaPedidos()
        {
            var Pedidolista = await _pedidoService.Lista();
            var clientes = await _clienteService.ObtenerNombre();
            List<VMPedido> vmPedidos = new List<VMPedido>();

            foreach (var item in Pedidolista)
            {
                vmPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = Convert.ToString(item.MontoTotal),
                    Estado = item.Estado,
                    FechaPedido = item.FechaPedido,
                    //NombresCompletos= clientes.Where(x => x.IdCliente ==item.IdCliente).FirstOrDefault().Nombres + "" +
                    //                  clientes.Where(x => x.IdCliente == item.IdCliente).FirstOrDefault().Apellidos
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
            var Pedidolista = await _pedidoService.Lista();
            var clientelista = await _clienteService.Lista();

            var idCliente = clientelista.Where(x => x.Dni == searchTerm).Select(x => x.IdCliente).FirstOrDefault();
            var estadoCliente =Pedidolista.Where(x => x.IdCliente ==idCliente).Select(x => x.Estado).FirstOrDefault();

            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pedidolista.Where(p =>p.IdCliente.Equals(idCliente) 
            );

            var MisPedidos = pedidosFiltrados.Where(p =>
            string.IsNullOrWhiteSpace(busqueda) || p.Estado.ToLower().Contains(busqueda.ToLower()) ||
            p.FechaPedido.Value.Date==(DateTime.TryParse(busqueda,out DateTime fechaBusqueda)?fechaBusqueda.Date:p.FechaPedido.Value.Date)
            );

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
                    FechaPedido = item.FechaPedido

                });
            }

            var clientePedido = pedidosFiltrados.FirstOrDefault().IdCliente;

            //var clientes = await _clienteService.ObtenerNombre();
            var clienteEncontrado = await _clienteService.ObtenerNombreCompleto(clientePedido);
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
        public async Task<IActionResult> ObtenerDetallePedido(string searchTerm = "", int page = 1, int itemsPerPage = 5)
        {
            var Pedidolista = await _pedidoService.Lista();
            var DetallePedidoLista = await _detallePedidoService.Lista();
            var Productos = await _productoService.Lista();
            var detallePedidosFiltrados = new List<DetallePedido>();
            int idPedido = 0 ;
            string codigo = "";
            List<string> nombresProductos = new List<string>();
            List<string> preciosProductos = new List<string>();

            // Filtro de búsqueda por término de búsqueda (searchTerm)
            if (searchTerm != null && searchTerm!="")
            {
                var pedidosFiltrados = Pedidolista.Where(p =>
                    string.IsNullOrWhiteSpace(searchTerm) || p.Codigo.ToLower().Contains(searchTerm.ToLower()
                    )
                );

                idPedido = pedidosFiltrados.First().IdPedido;
                codigo = pedidosFiltrados.First().Codigo;

                detallePedidosFiltrados = DetallePedidoLista.Where(p => p.IdPedido == idPedido).ToList();


                var clientePedido = pedidosFiltrados.First().IdCliente;
                var clientes = await _clienteService.ObtenerNombre();
                var clienteEncontrado = clientes.Where(x => x.IdCliente == clientePedido).First().Nombres + "" +
                                        clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;

                var productoPedido = detallePedidosFiltrados.Where(x => x.IdPedido == idPedido).Select(x => x.IdProducto).ToArray();
                var productos = await _productoService.ObtenerNombre();


                foreach (var item in productoPedido)
                {
                    var productoEncontrado = productos.Where(x => x.IdProducto == item).Select(x => x.Descripcion).FirstOrDefault();
                    if (productoEncontrado != null)
                    {
                        nombresProductos.Add(productoEncontrado);
                    }
                }

                foreach (var item in productoPedido)
                {
                    var productoEncontrado = productos.Where(x => x.IdProducto == item).Select(x => x.Precio).FirstOrDefault();
                    preciosProductos.Add(productoEncontrado.ToString());

                }
            }
            else
            {
                detallePedidosFiltrados = DetallePedidoLista;
                var productoPedido = detallePedidosFiltrados.Select(x => x.IdProducto).ToArray();
                var productos = await _productoService.ObtenerNombre();

                foreach (var item in productoPedido)
                {
                    var productoEncontrado = productos.Where(x => x.IdProducto == item).Select(x => x.Descripcion).FirstOrDefault();
                    if (productoEncontrado != null)
                    {
                        nombresProductos.Add(productoEncontrado);
                    }
                }

                foreach (var item in productoPedido)
                {
                    var productoEncontrado = productos.Where(x => x.IdProducto == item).Select(x => x.Precio).FirstOrDefault();
                    preciosProductos.Add(productoEncontrado.ToString());

                }
            }

            // Paginación
            var pedidosPaginados = detallePedidosFiltrados.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = detallePedidosFiltrados.Count(),codigos=codigo,precios=preciosProductos,nombresProducto=nombresProductos});
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleFinal(string searchTerm, int page = 1, int itemsPerPage = 3)
        {
            var Pedidolista = await _pedidoService.Lista();
            var DetallePedidoLista = await _detallePedidoService.Lista();
            int idPedido = 0,cantidadTotal = 0;
            string codigo = "";
            decimal precioTotal = 0;
            int[] product;
            List<VMDetallePedido> vmDetallePedido = new List<VMDetallePedido>();

            if (searchTerm != null && searchTerm != "")
            {
                var pedidosFiltrados = Pedidolista.Where(p => p.Codigo == searchTerm).ToList();

                idPedido = pedidosFiltrados.First().IdPedido;
                codigo = pedidosFiltrados.First().Codigo;

                var clientePedido = pedidosFiltrados.First().IdCliente;
                var clientes = await _clienteService.ObtenerNombre();
                var clienteEncontrado = clientes.Where(x => x.IdCliente == clientePedido).First().Nombres + "" +
                                        clientes.Where(x => x.IdCliente == clientePedido).First().Apellidos;

                var productoPedido = DetallePedidoLista.Where(x => x.IdPedido == idPedido).ToList();
                var productos = await _productoService.ObtenerNombre();
                product = productos.Select(x => x.IdProducto).ToArray();
                var categorias = await _categoriaService.ObtenerNombre();
                
              
                var productoID = DetallePedidoLista.Where(x => x.IdPedido == idPedido).Select(x=>x.IdProducto).ToArray();
                List<int> categoriasID = new List<int>();
                List<int> ProductosIdAgredados = new List<int>();
                List<decimal> precios = new List<decimal>();
                int[] categoriasProduct;
                decimal[] preciosProduct;



                for (int i = 0; i < productoID.Length; i++) {
                    var categoriasFinales = productos.Where(x => x.IdProducto == productoID[i]).Select(x => x.IdCategoria).FirstOrDefault();
                    categoriasID.Add(categoriasFinales);
                   
                }
                categoriasProduct = categoriasID.ToArray();

                for (int i = 0; i < productoID.Length; i++)
                {
                    var preciosFinales = productos.Where(x => x.IdProducto == productoID[i]).Select(x=>x.Precio).FirstOrDefault();
                    precios.Add(preciosFinales);

                }

                preciosProduct = precios.ToArray();

                var categoriaInicial = 0;
                for (int j = 0; j < preciosProduct.Length; j++)
                {

                    for (int i = 0; i < productoID.Length; i++)
                    {

                        bool ProductoAgregado = ProductosIdAgredados.Any(x => x == productoID[i]);
                        if (ProductoAgregado==false)
                        {
                            categoriaInicial = categoriasProduct[j];
                            var categoriasProductos = productos.Where(x => x.IdProducto == productoID[i] && x.Precio == preciosProduct[j]).Select(x => x.IdCategoria).FirstOrDefault();
                            var precioInicial = preciosProduct[j];
                            var preciosProductos = productos.Where(x => x.IdProducto == productoID[i] && x.IdCategoria == categoriasProductos).Select(x => x.Precio).FirstOrDefault();

                            var total = productoPedido.Where(x => x.IdProducto == productoID[i]).Select(x => x.Total).FirstOrDefault();
                            var cantidad = productoPedido.Where(x => x.IdProducto == productoID[i]).Select(x => x.Cantidad).FirstOrDefault();

                            if (precioInicial == preciosProductos && categoriaInicial == categoriasProductos)
                            {
                                precioTotal = precioTotal + Convert.ToDecimal(total);
                                cantidadTotal = cantidadTotal + Convert.ToInt32(cantidad);
                                ProductosIdAgredados.Add(productoID[i]);
                                
                                                           
                            }

                        }

                        if (i == productoID.Length - 1 && precioTotal>0 && cantidadTotal>0)
                        {
                            vmDetallePedido.Add(new VMDetallePedido
                            {
                                DescripcionProducto = productos.Where(x => x.IdProducto == productoID[j]).Select(x => x.Descripcion).FirstOrDefault(),
                                CategoriaProducto = categorias.Where(x => x.IdCategoria == categoriaInicial).Select(x => x.TipoDeCategoria).FirstOrDefault(),
                                Precio =Convert.ToString(productos.Where(x => x.IdProducto == productoID[j]).Select(x => x.Precio).FirstOrDefault()),
                                Total = Convert.ToString(precioTotal),
                                Cantidad = cantidadTotal,
                            });
                            precioTotal = 0;
                            cantidadTotal = 0;
                            ProductosIdAgredados.Add(productoID[j]);

                        }

                    }
                    
                    
                    }

                
            }

            // Paginación
            var pedidosPaginados = vmDetallePedido.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return StatusCode(StatusCodes.Status200OK, new { pedidos = pedidosPaginados, totalItems = vmDetallePedido.Count(), codigos = codigo});
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
            var direccion = clientes.Where(x => x.Dni == numeroDocumento).First().Direccion;
            var telefono = clientes.Where(x => x.Dni == numeroDocumento).First().Telefono;
            vmClientelista.Add(new VMCliente
            {
               IdCliente =idcliente,
               NombreCompleto=nombreCompletos,
               Direccion=direccion,
               Telefono=telefono
               
            });
            
            return StatusCode(StatusCodes.Status200OK, vmClientelista);
        }

        [HttpGet]
        public async Task<IActionResult> ListaNumeroDocumento(string nombreCompleto)
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var clientes = await _clienteService.ObtenerNombre();           
            var apellidos = clientes.FirstOrDefault(x => nombreCompleto.StartsWith(x.Apellidos)).Apellidos;
            var nombres = clientes.FirstOrDefault(x => nombreCompleto.EndsWith(x.Nombres)).Nombres;
             
             vmClientelista.Add(new VMCliente
             {
                 IdCliente = clientes.Where(x => x.Apellidos == apellidos && x.Nombres == nombres).First().IdCliente,
                 Dni = clientes.Where(x => x.Apellidos==apellidos && x.Nombres==nombres).First().Dni,
                 Direccion= clientes.Where(x => x.Apellidos == apellidos && x.Nombres == nombres).First().Direccion,
                 Telefono= clientes.Where(x => x.Apellidos == apellidos && x.Nombres == nombres).First().Telefono
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

        [HttpPost]
        public async Task<IActionResult> ActualizarPedido([FromBody] VMPedido modelo)
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
                        FechaPedido= Convert.ToDateTime(pedidoCreado.FechaPedido),
                        Estado=modelo.Estado,
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

        //[HttpPost]
        //public async Task<IActionResult> ActualizarDetallePedido([FromBody] VMDetallePedido modelo,int IdPedido)
        //{
        //    GenericResponse<VMPedido> gResponse = new GenericResponse<VMPedido>();

        //    try
        //    {
        //        List<DetallePedido> listaPedidos = new List<DetallePedido>();
        //        List<VMDetallePedido> listaVMDetallePedido = new List<VMDetallePedido>();
        //        //List<DetallePedido> detallePedido = new List<DetallePedido>();
        //        var clientes = await _clienteService.ObtenerNombre();
        //        if (modelo != null)
        //        {
        //            listaVMDetallePedido.Add(modelo);
        //            foreach (var item in listaVMDetallePedido)
        //            {
        //                listaPedidos.Add(new DetallePedido
        //                {
        //                    IdPedido = IdPedido,
        //                    IdProducto = item.IdProducto,
        //                    Cantidad = item.Cantidad,
        //                    Total = Convert.ToDecimal(item.Total)
        //                });
        //            }

        //         }
        //            //}
                

        //        DetallePedido pedidoCreado = await _pedidoService.(listaPedidos.First());

        //        List<VMPedido> vmPedidolista = new List<VMPedido>();
        //        List<Pedidos> listPedidos = new List<Pedidos>();
        //        if (pedidoCreado != null)
        //        {
        //            //listPedidos.Add(pedidoCreado);
        //            //foreach (var item in listPedidos)
        //            //{
        //            vmPedidolista.Add(new VMPedido
        //            {

        //                MontoTotal = Convert.ToString(pedidoCreado.MontoTotal),
        //                FechaPedido = pedidoCreado.FechaPedido,
        //                Estado = pedidoCreado.Estado,
        //                DetallePedido = pedidoCreado.DetallePedido.Select(detalle => new VMDetallePedido
        //                {
        //                    IdProducto = detalle.IdProducto,
        //                    Cantidad = detalle.Cantidad,
        //                    Total = Convert.ToString(detalle.Total)
        //                }).ToList()
        //            });
        //            //}
        //        }

        //        gResponse.Estado = true;
        //        gResponse.objeto = vmPedidolista.First();

        //    }
        //    catch (Exception ex)
        //    {
        //        gResponse.Estado = false;
        //        gResponse.Mensaje = ex.Message;

        //    }

        //    return StatusCode(StatusCodes.Status200OK, gResponse);

        //}

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