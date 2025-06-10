using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashBoardServicio;
        private readonly IDashBoardServiceCliente _dashBoardServicioCliente;
        public DashBoardController(IDashBoardService dashBoardServicio, IDashBoardServiceCliente dashBoardServicioCliente)
        {
            _dashBoardServicio = dashBoardServicio;
            _dashBoardServicioCliente = dashBoardServicioCliente;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DashBoardCliente()
        {
            return View();
        }

        public IActionResult DashBoard()
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
                vmDashboard.TotalIngresos = await _dashBoardServicio.TotalIngresosDelDia();
                vmDashboard.TotalProductos = await _dashBoardServicio.TotalProductos();
                vmDashboard.TotalCategorias = await _dashBoardServicio.TotalCategorias();
                vmDashboard.TotalDeLatas = await _dashBoardServicio.TotalDeLatas();

                List<VMPedidosSemana> listaVentasSemana = new List<VMPedidosSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, decimal?> item in await _dashBoardServicio.PedidosUltimaSemana())
                {
                    listaVentasSemana.Add(new VMPedidosSemana()
                    {
                        Fecha = item.Key,
                        MontoTotal = item.Value
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


        [HttpGet]
        public async Task<IActionResult> ObtenerResumenCliente(string dni,string correo)
        {
            GenericResponse<VMDashBoardCliente> gResponse = new GenericResponse<VMDashBoardCliente>();

            try
            {
                VMDashBoardCliente vmDashboardCliente = new VMDashBoardCliente();
                vmDashboardCliente.TotalDeMisPedidos = await _dashBoardServicioCliente.TotalDeMisPedidos(correo);
                //vmDashboard.TotalIngresos = await _dashBoardServicio.TotalIngresosUltimaSemana();
                vmDashboardCliente.TotalDeMisPagos = await _dashBoardServicioCliente.TotalDeMisPagos(dni);
                vmDashboardCliente.TotalDeMisMensajes = await _dashBoardServicioCliente.TotalDeMisMensajes(dni);

                List<VMPagosSemana> listaVentasSemana = new List<VMPagosSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string,decimal?> item in await _dashBoardServicioCliente.PagosUltimaSemana(dni))
                {
                    listaVentasSemana.Add(new VMPagosSemana()
                    {
                        FechaPago = item.Key,
                        MontoTotalDePago = item.Value
                    });

                }

                foreach (KeyValuePair<string, int> item in await _dashBoardServicioCliente.MisProductosTopUltimaSemana(dni))
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });

                }

                vmDashboardCliente.PagosUltimaSemana = listaVentasSemana;
                vmDashboardCliente.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.objeto = vmDashboardCliente;

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
