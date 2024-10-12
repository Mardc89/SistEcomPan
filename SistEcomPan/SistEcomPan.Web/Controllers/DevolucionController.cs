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
        private readonly IMensajeService _mensajeService;
        private readonly IClienteService _clienteService;

        public DevolucionController(IMensajeService _mensajeService,IClienteService _clienteService)
        {
                
        }
        public IActionResult Index()
        {
            return View();
        }

    //    [HttpPost]
    //    public async Task<IActionResult> Crear([FromBody] VMDevolucion modelo)
    //    {
    //        GenericResponse<VMPedido> gResponse = new GenericResponse<VMPedido>();

    //        try
    //        {
    //            List<Devolucion> listaDevolucion = new List<Devolucion>();
    //            List<VMPedido> listaVMPedidos = new List<VMPedido>();
    //            var clientes = await _clienteService.ObtenerNombre();
    //            if (modelo != null)
    //            {
    //                listaDevolucion.Add(new Devolucion
    //                {
    //                    IdPedido =modelo.IdPedido,
    //                    PrecioDelProducto=modelo.PrecioDelProducto,
    //                    CantidadDevolucion=modelo.CantidadDevolucion,
    //                    Total=modelo.Total
                       
    //                });
                   
    //            }

    //            Pedidos pedidoCreado = await _pedidoService.Registrar(listaPedidos.First());

    //            List<VMPedido> vmPedidolista = new List<VMPedido>();
    //            List<Pedidos> listPedidos = new List<Pedidos>();
    //            if (pedidoCreado != null)
    //            {
    //                //listPedidos.Add(pedidoCreado);
    //                //foreach (var item in listPedidos)
    //                //{
    //                vmPedidolista.Add(new VMDevolucion
    //                {
    //                    IdPedido = pedidoCreado.IdPedido,
    //                    IdCliente = pedidoCreado.IdCliente,
    //                    Codigo = pedidoCreado.Codigo,
    //                    MontoTotal = Convert.ToString(pedidoCreado.MontoTotal),
    //                    FechaPedido = pedidoCreado.FechaPedido,
    //                    Estado = pedidoCreado.Estado,
    //                });
    //                //}
    //            }

    //            gResponse.Estado = true;
    //            gResponse.objeto = vmPedidolista.First();

    //        }
    //        catch (Exception ex)
    //        {
    //            gResponse.Estado = false;
    //            gResponse.Mensaje = ex.Message;

    //        }

    //        return StatusCode(StatusCodes.Status200OK, gResponse);

    //    }
    }
}
