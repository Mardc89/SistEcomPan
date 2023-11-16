using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;

namespace SistEcomPan.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IHostEnvironment _environment;
        

        public UsuarioController(IUsuarioService usuarioServicio ,IHostEnvironment environment)
        {
            _usuarioServicio = usuarioServicio;
            _environment=environment;
           

    }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crear([FromForm]IFormFile foto, [FromForm] Usuarios usuario)
        {
            try
            {
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    usuario.NombreFoto = foto.FileName;

                    var path = Path.Combine(_environment.ContentRootPath, "Imagenes");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path, foto.FileName);

                    string nombreCodigo=Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                 
                    usuario.UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        foto.CopyTo(stream);

                    }
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                Usuarios usuarioCreado=await _usuarioServicio.Crear(usuario);
                return BadRequest();

            }
            catch (Exception)
            {

                return BadRequest();
            }
            
        }
    }
}
