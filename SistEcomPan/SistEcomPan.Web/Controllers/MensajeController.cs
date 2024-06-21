using Microsoft.AspNetCore.Mvc;

namespace SistEcomPan.Web.Controllers
{
    public class MensajeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
