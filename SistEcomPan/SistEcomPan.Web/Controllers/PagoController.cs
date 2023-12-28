using Microsoft.AspNetCore.Mvc;

namespace SistEcomPan.Web.Controllers
{
    public class PagoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NuevoPago()
        {
            return View();
        }
    }
}
