using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;

namespace SistEcomPan.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarios; 

        public UsuarioController(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crear([FromForm]IFormFile foto, [FromForm] string modelo)
        {
            try
            {
                string nombreFoto = "";
                

                if(foto != null)
                {
                    string nombreEnCodigo=Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto=string.Concat(nombreEnCodigo, extension);
                   

                }

                Usuarios usuarioCreado = await _usuarios.Crear();
                

            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
    }
}
