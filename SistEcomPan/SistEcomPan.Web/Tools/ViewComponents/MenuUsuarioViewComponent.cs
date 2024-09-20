using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistEcomPan.Web.Tools.ViewComponents
{
    public class MenuUsuarioViewComponent:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string nombreUsuario = "";
            string nombreFotoUsuario = "";
            string dniUsuario = "";
            string nombreCompleto = "";
            string correo = "";

            if (claimUser.Identity.IsAuthenticated){
                nombreUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                nombreFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("NombreFoto").Value;
                correo = ((ClaimsIdentity)claimUser.Identity).FindFirst("Correo").Value;
                dniUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("Dni").Value;
                nombreCompleto = ((ClaimsIdentity)claimUser.Identity).FindFirst("NombreCompleto").Value;
            }

            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["nombreFotoUsuario"] = nombreFotoUsuario;
            ViewData["nombreCompleto"] = nombreCompleto;
            ViewData["dniUsuario"] = dniUsuario;
            ViewData["correo"] = correo;

            return View();

        } 
    }
}
