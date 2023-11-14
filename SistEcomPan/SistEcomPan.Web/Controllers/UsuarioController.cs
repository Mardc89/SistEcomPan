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
        private readonly IEncriptService _encriptservice;

        public UsuarioController(IGenericRepository<Usuarios> usuarios ,IHostEnvironment environment, IEncriptService encriptservice)
        {
            _usuarios = usuarios;
            _environment=environment;
            _encriptservice=encriptservice;

    }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crear([FromForm]IFormFile foto, [FromForm] Usuarios usuario)
        {
            try
            {

                if (foto != null && foto.Length > 0)
                {
                    usuario.NombreFoto = foto.FileName;

                    var path = Path.Combine(_environment.ContentRootPath, "Imagenes");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullpath = Path.Combine(path, foto.FileName);

                    usuario.UrlFoto = fullpath;
                    string ClaveGenerada = _encriptservice.GenerarClave();
                    usuario.Clave = _encriptservice.ConvertirSha256(ClaveGenerada);
                                      
                    bool respuesta=await _usuarios.Guardar(usuario);

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        foto.CopyTo(stream);

                    }

                    return Ok(foto);
                }

                return BadRequest();

            }
            catch (Exception)
            {

                return BadRequest();
            }
            
        }
    }
}
