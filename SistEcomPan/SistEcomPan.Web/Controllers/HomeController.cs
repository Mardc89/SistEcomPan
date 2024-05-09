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

namespace SistEcomPan.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IClienteService _clienteServicio;

        public HomeController(IUsuarioService usuarioServicio, IRolService rolServicio, IClienteService clienteServicio)
        {
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
            _clienteServicio = clienteServicio;
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
                    Nombres = usuarioEncontrado.Nombres,
                    Apellidos = usuarioEncontrado.Apellidos,
                    Correo = usuarioEncontrado.Correo,
                    NombreUsuario = usuarioEncontrado.NombreUsuario,
                    UrlFoto = usuarioEncontrado.UrlFoto,
                    NombreRol = RolesUsuarios.Where(x => x.IdRol == usuarioEncontrado.IdRol).Select(x => x.NombreRol).First()
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

        [HttpGet]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                List<Usuarios> entidadUsuario = new List<Usuarios>();

                entidadUsuario.Add(new Usuarios
                {
                    IdUsuario = modelo.IdUsuario,
                    Dni = modelo.Dni,
                    Nombres = modelo.Nombres,
                    Apellidos = modelo.Apellidos,
                    Correo = modelo.Correo,
                    NombreUsuario = modelo.NombreUsuario,
                    Clave = modelo.Clave,
                    UrlFoto = modelo.UrlFoto
                });

                entidadUsuario.ElementAt(0).IdUsuario = int.Parse(idUsuario);
                bool resultado = await _usuarioServicio.GuardarPerfil(entidadUsuario.First());
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

                vmCliente.Add(new VMCliente
                {
                    IdCliente = usuarioEncontrado.IdCliente,
                    Nombres = usuarioEncontrado.Nombres,
                    Apellidos = usuarioEncontrado.Apellidos,
                    Correo = usuarioEncontrado.Correo,
                    NombreUsuario = usuarioEncontrado.NombreUsuario,
                    UrlFoto = usuarioEncontrado.UrlFoto
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

        [HttpGet]
        public async Task<IActionResult> GuardarPerfilCliente([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claiumUser = HttpContext.User;

                string idUsuario = claiumUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                List<Usuarios> entidadUsuario = new List<Usuarios>();

                entidadUsuario.Add(new Usuarios
                {
                    IdUsuario = modelo.IdUsuario,
                    Dni = modelo.Dni,
                    Nombres = modelo.Nombres,
                    Apellidos = modelo.Apellidos,
                    Correo = modelo.Correo,
                    NombreUsuario = modelo.NombreUsuario,
                    Clave = modelo.Clave,
                    UrlFoto = modelo.UrlFoto
                });

                entidadUsuario.ElementAt(0).IdUsuario = int.Parse(idUsuario);
                bool resultado = await _usuarioServicio.GuardarPerfil(entidadUsuario.First());
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
            return RedirectToAction("Login","Acceso");

        }
    }
}