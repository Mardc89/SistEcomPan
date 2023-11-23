using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using Negocio.Implementacion;
using SistEcomPan.Web.Models.ViewModels;
using Newtonsoft.Json;

namespace SistEcomPan.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IHostEnvironment _environment;
        private readonly IRolService _rolService;

        public UsuarioController(IUsuarioService usuarioServicio ,IHostEnvironment environment,IRolService rolService)
        {
            _usuarioServicio = usuarioServicio;
            _environment=environment;
            _rolService = rolService;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolService.lista();
            List<VMRol> vmListaRoles = new List<VMRol>();
            foreach (var item in lista)
            {
                vmListaRoles.Add(new VMRol
                {
                    IdRol = item.IdRol,
                    NombreRol=item.NombreRol
                }); 
            }
            return StatusCode(StatusCodes.Status200OK,vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var Usuariolista = await _usuarioServicio.Lista();
            List<VMUsuario> vmUsuariolista = new List<VMUsuario>();
            foreach (var item in Usuariolista)
            {
                vmUsuariolista.Add(new VMUsuario
                {
                    IdUsuario = item.IdUsuario,
                    NombreRol = item.NombreUsuario
                });
            }
            return StatusCode(StatusCodes.Status200OK,new { data = vmUsuariolista });
        }


        public async Task<IActionResult> Crear([FromForm]IFormFile foto, [FromForm] string modelo)
        {
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    NombreFoto = foto.FileName;

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
                 
                    string UrlFoto = fullpath;

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
