using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;

namespace SistEcomPan.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IGenericRepository<Usuarios> _usuarios;

        public UsuarioController(IGenericRepository<Usuarios> usuarios)
        {
            _usuarios = usuarios;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crear([FromForm]IFormFile foto, [FromForm] string modelo)
        {
            return View();
        }
    }
}
