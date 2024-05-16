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

            if (claimUser.Identity.IsAuthenticated){
                nombreUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                nombreFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("NombreFoto").Value;
                dniUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("Dni").Value;
            }

            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["nombreFotoUsuario"] = nombreFotoUsuario;
            ViewData["dniUsuario"] = dniUsuario;

            return View();

        } 
    }
}
