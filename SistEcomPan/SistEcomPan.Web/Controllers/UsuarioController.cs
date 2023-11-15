using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;

namespace SistEcomPan.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IGenericRepository<Usuarios> _usuarios;
        private readonly IHostEnvironment _environment;
        

        public UsuarioController(IGenericRepository<Usuarios> usuarios ,IHostEnvironment environment)
        {
            _usuarios = usuarios;
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

                    usuario.NombreFoto = NombreFoto;                   
                    usuario.UrlFoto = fullpath;

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        foto.CopyTo(stream);

                    }


                    return Ok(foto);
                }
                                 
                bool respuesta=await _usuarios.Guardar(usuario);
                return BadRequest();

            }
            catch (Exception)
            {

                return BadRequest();
            }
            
        }
    }
}
