using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashBoardServicio;
        public DashBoardController(IDashBoardService dashBoardServicio)
        {
            _dashBoardServicio = dashBoardServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();

            try
            {
                VMDashBoard vmDashboard = new VMDashBoard();
                vmDashboard.TotalPedidos = await _dashBoardServicio.TotalPedidosUltimaSemana();
                vmDashboard.TotalIngresos = await _dashBoardServicio.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashBoardServicio.TotalProductos();
                vmDashboard.TotalCategorias = await _dashBoardServicio.TotalCategorias();

                List<VMPedidosSemana> listaVentasSemana = new List<VMPedidosSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashBoardServicio.PedidosUltimaSemana())
                {
                    listaVentasSemana.Add(new VMPedidosSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });

                }

                foreach (KeyValuePair<string, int> item in await _dashBoardServicio.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });

                }

                vmDashboard.PedidosUltimaSemana = listaVentasSemana;
                vmDashboard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.objeto = vmDashboard;

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
