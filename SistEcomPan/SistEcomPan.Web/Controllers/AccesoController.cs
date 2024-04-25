using Microsoft.AspNetCore.Mvc;

using SistEcomPan.Web.Models.ViewModels;
using Negocio.Interfaces;
using Entidades;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace SistEcomPan.Web.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IClienteService _clienteServicio;

        public AccesoController(IUsuarioService usuarioServicio,IClienteService clienteServicio)
        {
            _usuarioServicio= usuarioServicio;
            _clienteServicio= clienteServicio;
                
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated){
                return RedirectToAction("Index", "Home");
            }

            return View();
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
                new Claim(ClaimTypes.Role,usuarioEncontrado.IdRol.ToString()),
                new Claim("UrlFoto",usuarioEncontrado.UrlFoto),
                new Claim("Dni",usuarioEncontrado.Dni)
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
            else if (clienteEncontrado != null)
            {
                List<Claim> claims = new List<Claim>(){
                new Claim(ClaimTypes.Name,clienteEncontrado.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier,clienteEncontrado.IdCliente.ToString()),
                new Claim("UrlFoto",clienteEncontrado.UrlFoto),
                new Claim("Dni",clienteEncontrado.Dni)
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
    }
}
