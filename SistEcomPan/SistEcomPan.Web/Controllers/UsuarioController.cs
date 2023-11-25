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
        private readonly IRolService _rolService;

        public UsuarioController(IUsuarioService usuarioServicio,IRolService rolService)
        {
            _usuarioServicio = usuarioServicio;
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

        public async Task <List<VMUsuario>> ListaUsuarios()
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
            return vmUsuariolista;
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
                    string nombreCodigo=Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                 
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                var Usuariolista = await _usuarioServicio.Lista();


                List<Usuarios> Usuariolistas = new List<Usuarios>();
                List<VMUsuario> Usuariolist = new List<VMUsuario>();
                if (vmUsuario != null)
                {
                    Usuariolist.Add(vmUsuario);
                    foreach (var item in Usuariolist)
                    {
                        Usuariolistas.Add(new Usuarios
                        {
                            IdUsuario = item.IdUsuario,
                            NombreUsuario = item.Nombre,
                            Estado = Convert.ToBoolean(item.EsActivo),

                        });
                    }
                }

                Usuarios usuarioCreado=await _usuarioServicio.Crear(Usuariolistas.First(),fotoStream,NombreFoto,urlPlantillaCorreo);
                return BadRequest();

            }
            catch (Exception)
            {

                return BadRequest();
            }
            
        }
    }
}
