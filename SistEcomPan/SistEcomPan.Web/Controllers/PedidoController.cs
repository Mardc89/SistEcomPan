
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

        public PedidoController(IPedidoService pedidoService,IClienteService clienteService)
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _pedidoService.lista();
            List<VMPedido> vmListaPedidos = new List<VMPedido>();
            var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in lista)
            {
                vmListaPedidos.Add(new VMPedido
                {
                    IdPedido = item.IdPedido,
                    IdCliente = item.IdCliente,
                    Codigo = item.Codigo,
                    MontoTotal = item.MontoTotal,
                    Estado = item.Estado,
                    NombresCompletos= clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +
                    clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos
                }) ;
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaPedidos });
        }

        [HttpGet]
        public async Task<IActionResult> ListaClientes()
        {
            var Clientelista = await _clienteService.Lista();
            List<VMCliente> vmClientelista = new List<VMCliente>();
            var clientes = await _clienteService.ObtenerNombre();
            foreach (var item in Clientelista)
            {
                vmClientelista.Add(new VMCliente
                {
                    IdCliente=item.IdCliente,
                    NombreCompleto = clientes.Where(x => x.IdCliente == item.IdCliente).First().Nombres +
                    clientes.Where(x => x.IdCliente == item.IdCliente).First().Apellidos

                });
            }
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
                if (modelo != null)
                {
                    listaVMPedidos.Add(modelo);
                    foreach (var item in listaVMPedidos)
                    {
                        listaPedidos.Add(new Pedidos
                        {
                            IdPedido = item.IdPedido,
                            IdCliente = item.IdCliente,
                            Codigo = item.Codigo,
                            MontoTotal = item.MontoTotal,
                            FechaPedido = item.FechaPedido,
                            Estado = item.Estado,
                            DetallePedido = item.VMDetallePedido.Select(detalle => new DetallePedido
                            {                             
                              IdProducto=detalle.IdProducto,
                              Cantidad=detalle.Cantidad,


                            }).ToList()

                        }); 
                    }
                }

                Pedidos categoriaCreada = await _pedidoService.Registrar(listaPedidos.First());

                List<VMCategoria> vmCategorialista = new List<VMCategoria>();
                List<Categorias> listCategorias = new List<Categorias>();
                if (categoriaCreada != null)
                {
                    listCategorias.Add(categoriaCreada);


                    foreach (var item in listCategorias)
                    {
                        vmCategorialista.Add(new VMCategoria
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria = item.TipoDeCategoria,
                            Estado = Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmCategorialista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {

                List<Categorias> listaCategorias = new List<Categorias>();
                List<VMCategoria> listaVMCategorias = new List<VMCategoria>();
                if (modelo != null)
                {
                    listaVMCategorias.Add(modelo);
                    foreach (var item in listaVMCategorias)
                    {
                        listaCategorias.Add(new Categorias
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria = item.TipoDeCategoria,
                            Estado = Convert.ToBoolean(item.Estado)
                        });
                    }
                }

                Categorias categoriaEditada = await _categoriaService.Editar(listaCategorias.First());

                List<Categorias> listCategorias = new List<Categorias>();
                List<VMCategoria> vmCategorialista = new List<VMCategoria>();
                if (categoriaEditada != null)
                {
                    listCategorias.Add(categoriaEditada);
                    foreach (var item in listCategorias)
                    {
                        vmCategorialista.Add(new VMCategoria
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria = item.TipoDeCategoria,
                            Estado = Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmCategorialista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdCategoria)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _categoriaService.Eliminar(IdCategoria);

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