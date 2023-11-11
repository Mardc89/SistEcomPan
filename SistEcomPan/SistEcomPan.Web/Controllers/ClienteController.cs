using Microsoft.AspNetCore.Mvc;

namespace SistEcomPan.Web.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
