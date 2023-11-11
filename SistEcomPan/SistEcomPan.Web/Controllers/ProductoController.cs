using Microsoft.AspNetCore.Mvc;

namespace SistEcomPan.Web.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
