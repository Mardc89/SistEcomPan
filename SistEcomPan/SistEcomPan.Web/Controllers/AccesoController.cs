using Microsoft.AspNetCore.Mvc;

using SistEcomPan.Web.Models.ViewModels;
using Negocio.Interfaces;
using Entidades;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Negocio.Implementacion;
using Newtonsoft.Json;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IClienteService _clienteServicio;
        private readonly IRolService _rolServicio;
        private readonly IEncriptService _encriptServicio;

        public AccesoController(IUsuarioService usuarioServicio,IClienteService clienteServicio,IRolService rolServicio)
        {
            _usuarioServicio= usuarioServicio;
            _clienteServicio= clienteServicio;
            _rolServicio = rolServicio;
                
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated){
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult RestablecerClave()
        {
            return View();
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginCliente(VMCliente modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {

                string NombreFoto = "";
                Stream fotoStream = null;
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                var Usuariolista = await _usuarioServicio.Lista();


                List<Clientes> listaClientes = new List<Clientes>();
                List<VMCliente> listaVMClientes = new List<VMCliente>();
                if (modelo != null)
                {
                    listaVMClientes.Add(modelo);
                    foreach (var item in listaVMClientes)
                    {
                        listaClientes.Add(new Clientes
                        {
                            IdCliente = item.IdCliente,
                            TipoCliente=item.TipoCliente,
                            Dni = item.Dni,
                            Nombres = item.Nombres,
                            Apellidos = item.Apellidos,
                            Correo = item.Correo,
                            Direccion=item.Direccion,
                            Telefono=item.Telefono,
                            IdDistrito = item.IdDistrito,
                            NombreUsuario = item.NombreUsuario,
                            Clave = _encriptServicio.DesencriptarPassword(item.Clave),
                            Estado =true,
                            UrlFoto=""
                        });
                    }
                }

                Clientes clienteCreado = await _clienteServicio.Crear(listaClientes.First(),fotoStream,NombreFoto, urlPlantillaCorreo);



            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return RedirectToAction("Login", "Acceso");
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Usuarios usuarioEncontrado = await _usuarioServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            Clientes clienteEncontrado = await _clienteServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            if (usuarioEncontrado==null && clienteEncontrado==null)
            {
                ViewData["Mensaje"] = "El Usuario no Existe";
                return View();
            }

            ViewData["Mensaje"] = null;

            if (usuarioEncontrado != null)
            {

                List<Claim> claims = new List<Claim>(){
                new Claim(ClaimTypes.Name,usuarioEncontrado.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier,usuarioEncontrado.IdUsuario.ToString()),
                new Claim("NombreFoto",usuarioEncontrado.NombreFoto),
                new Claim("Dni",usuarioEncontrado.Dni)
                };

                var UsuarioRoles = await _rolServicio.ObtenerNombre();
                var nombreRoles = UsuarioRoles.Where(x => x.IdRol == usuarioEncontrado.IdRol).Select(x => x.NombreRol).First();
                claims.Add(new Claim(ClaimTypes.Role, nombreRoles));

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelo.MantenerSesion
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                 );
            }
            else if (clienteEncontrado != null)
            {
                List<Claim> claims = new List<Claim>(){
                new Claim(ClaimTypes.Name,clienteEncontrado.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier,clienteEncontrado.IdCliente.ToString()),
                new Claim("NombreFoto",clienteEncontrado.NombreFoto),
                new Claim("Dni",clienteEncontrado.Dni),
                new Claim(ClaimTypes.Role,"Cliente")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelo.MantenerSesion
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                 );
            }

            return RedirectToAction("Index","Home");
        }


        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {

                string UrlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioServicio.RestablecerClave(modelo.Clave,modelo.Correo,UrlPlantillaCorreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo,su contraseña fue restablecida.Revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["MensajeError"] = "Intentelo de nuevo mas tarde.";
                    ViewData["MensajeError"] = null;
                }

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["MensajeError"] = null;

            }
            return View();
        }
    }
}
