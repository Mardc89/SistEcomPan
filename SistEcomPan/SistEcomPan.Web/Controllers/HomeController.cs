using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistEcomPan.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;
using Negocio.Interfaces;
using Entidades;
using Newtonsoft.Json;

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IClienteService _clienteServicio;
        private readonly IDistritoService _distritoServicio;
        private readonly IEncriptService _EncriptarServicio;

        public HomeController(IUsuarioService usuarioServicio, IRolService rolServicio, IClienteService clienteServicio, IDistritoService distritoServicio, IEncriptService encriptarServicio)
        {
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
            _clienteServicio = clienteServicio;
            _distritoServicio = distritoServicio;
            _EncriptarServicio = encriptarServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult PerfilUsuario()
        {
            return View();
        }
        public IActionResult PerfilCliente()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c=>c.Value).SingleOrDefault();

                Usuarios usuarioEncontrado = await _usuarioServicio.ObtenerPorId(int.Parse(idUsuario));

                List<VMUsuario> vmUsuario = new List<VMUsuario>();
                var RolesUsuarios= await _rolServicio.ObtenerNombre();
              

                vmUsuario.Add(new VMUsuario
                {
                    IdUsuario = usuarioEncontrado.IdUsuario,
                    Dni=usuarioEncontrado.Dni,
                    Nombres = usuarioEncontrado.Nombres,
                    Apellidos = usuarioEncontrado.Apellidos,
                    Correo = usuarioEncontrado.Correo,
                    Clave = _EncriptarServicio.DesencriptarPassword(usuarioEncontrado.Clave),
                    NombreUsuario = usuarioEncontrado.NombreUsuario,
                    NombreFoto = usuarioEncontrado.NombreFoto,
                    NombreRol = RolesUsuarios.Where(x => x.IdRol == usuarioEncontrado.IdRol).Select(x => x.NombreRol).First(),
                    IdRol=usuarioEncontrado.IdRol

                }); 

                response.Estado = true;
                response.objeto = vmUsuario.First();
                
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();

                }
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                List<Usuarios> entidadUsuario = new List<Usuarios>();

                if (vmUsuario != null)
                {
                    entidadUsuario.Add(new Usuarios
                    {
                        IdUsuario = vmUsuario.IdUsuario,
                        Dni = vmUsuario.Dni,
                        Nombres = vmUsuario.Nombres,
                        Apellidos = vmUsuario.Apellidos,
                        Correo = vmUsuario.Correo,
                        NombreUsuario = vmUsuario.NombreUsuario,
                        Clave = vmUsuario.Clave,
                        UrlFoto = vmUsuario.UrlFoto,
                        NombreFoto = vmUsuario.NombreFoto,
                        IdRol = vmUsuario.IdRol
                    });
                }
                entidadUsuario.ElementAt(0).IdUsuario = int.Parse(idUsuario);
                bool resultado = await _usuarioServicio.GuardarPerfil(entidadUsuario.First(),fotoStream,NombreFoto);
                response.Estado = resultado;
                
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                bool resultado = await _usuarioServicio.CambiarClave(
                    int.Parse(idUsuario),
                    modelo.claveActual,
                    modelo.claveNueva                  
                );
                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        public async Task<IActionResult> ObtenerCliente()
        {
            GenericResponse<VMCliente> response = new GenericResponse<VMCliente>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idCliente = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Clientes usuarioEncontrado = await _clienteServicio.ObtenerPorId(int.Parse(idCliente));

                List<VMCliente> vmCliente = new List<VMCliente>();
                var RolesUsuarios = await _rolServicio.ObtenerNombre();
                var NombresDistritos = await _distritoServicio.ObtenerNombre();

                vmCliente.Add(new VMCliente
                {
                    IdCliente = usuarioEncontrado.IdCliente,
                    IdDistrito=usuarioEncontrado.IdDistrito,
                    TipoCliente = usuarioEncontrado.TipoCliente,
                    Dni = usuarioEncontrado.Dni,
                    Nombres = usuarioEncontrado.Nombres,
                    Apellidos = usuarioEncontrado.Apellidos,
                    Direccion=usuarioEncontrado.Direccion,
                    Correo = usuarioEncontrado.Correo,
                    NombreUsuario = usuarioEncontrado.NombreUsuario,
                    Clave = _EncriptarServicio.DesencriptarPassword(usuarioEncontrado.Clave),
                    Telefono = usuarioEncontrado.Telefono,
                    NombreDistrito = NombresDistritos.Where(x => x.IdDistrito == usuarioEncontrado.IdDistrito).Select(x => x.NombreDistrito).First(),
                    UrlFoto = usuarioEncontrado.UrlFoto,
                    NombreFoto=usuarioEncontrado.NombreFoto
                    
                }); 

                response.Estado = true;
                response.objeto = vmCliente.First();

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfilCliente([FromForm] IFormFile foto,[FromForm] string modelo)
        {
            GenericResponse<VMCliente> response = new GenericResponse<VMCliente>();
            try
            {
                VMCliente vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();

                }

                ClaimsPrincipal claiumUser = HttpContext.User;

                string idCliente = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                List<Clientes> entidadCliente = new List<Clientes>();

                entidadCliente.Add(new Clientes
                {
                    IdCliente = vmCliente.IdCliente,
                    Dni =vmCliente.Dni,
                    TipoCliente=vmCliente.TipoCliente,
                    Nombres = vmCliente.Nombres,
                    Apellidos = vmCliente.Apellidos,
                    Direccion=vmCliente.Direccion,
                    Telefono=vmCliente.Telefono,
                    IdDistrito=vmCliente.IdDistrito,
                    Correo = vmCliente.Correo,
                    NombreUsuario = vmCliente.NombreUsuario,
                    Clave = vmCliente.Clave,
                    UrlFoto = vmCliente.UrlFoto
                });

                entidadCliente.ElementAt(0).IdCliente = int.Parse(idCliente);
                bool resultado = await _clienteServicio.GuardarPerfil(entidadCliente.First(), fotoStream, NombreFoto);
                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> CambiarClaveCliente([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                bool resultado = await _usuarioServicio.CambiarClave(
                    int.Parse(idUsuario),
                    modelo.claveActual,
                    modelo.claveNueva
                );
                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Acceso");

        }
    }
}