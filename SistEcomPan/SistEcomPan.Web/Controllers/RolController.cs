using Entidades;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;
using System.Runtime.CompilerServices;

namespace SistEcomPan.Web.Controllers
{
    public class RolController : Controller
    {
        private readonly IRolService _RolService;

        public RolController(IRolService RolService)
        {
            _RolService = RolService;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _RolService.Lista();
            List<VMRol> vmListaRoles = new List<VMRol>();
            foreach (var item in lista)
            {
                vmListaRoles.Add(new VMRol
                {
                    IdRol = item.IdRol,
                    NombreRol=item.NombreRol,
                    Estado = Convert.ToInt32(item.Estado)

                });
            }
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var Rollista = await _RolService.Lista();
            List<VMRol> vmRollista = new List<VMRol>();
            foreach (var item in Rollista)
            {
                vmRollista.Add(new VMRol
                {
                    IdRol=item.IdRol,
                    NombreRol=item.NombreRol,
                    Estado = Convert.ToInt32(item.Estado)

                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmRollista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMRol modelo)
        {
            GenericResponse<VMRol> gResponse = new GenericResponse<VMRol>();

            try
            {
                List<Roles> listaRoles = new List<Roles>();
                List<VMRol> listaVMRoles = new List<VMRol>();
                if (modelo != null)
                {
                    listaVMRoles.Add(modelo);
                    foreach (var item in listaVMRoles)
                    {
                        listaRoles.Add(new Roles
                        {
                            IdRol=item.IdRol,
                            NombreRol=item.NombreRol,
                            Estado=Convert.ToBoolean(item.Estado)
                        });
                    }
                }

                Roles rolCreado = await _RolService.Crear(listaRoles.First());

                List<VMRol> vmRoleslista = new List<VMRol>();
                List<Roles> listRoles = new List<Roles>();
                if (rolCreado != null)
                {
                    listRoles.Add(rolCreado);


                    foreach (var item in listRoles)
                    {
                        vmRoleslista.Add(new VMRol
                        {
                            IdRol=item.IdRol,
                            NombreRol=item.NombreRol,
                            Estado = Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmRoleslista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMRol modelo)
        {
            GenericResponse<VMRol> gResponse = new GenericResponse<VMRol>();

            try
            {

                List<Roles> listaRoles = new List<Roles>();
                List<VMRol> listaVMRoles = new List<VMRol>();
                if (modelo != null)
                {
                    listaVMRoles.Add(modelo);
                    foreach (var item in listaVMRoles)
                    {
                        listaRoles.Add(new Roles
                        {
                            IdRol = item.IdRol,
                            NombreRol = item.NombreRol,
                            Estado = Convert.ToBoolean(item.Estado)
                        });
                    }
                }

                Roles RolEditado = await _RolService.Editar(listaRoles.First());

                List<Roles> listRoles = new List<Roles>();
                List<VMRol> vmRolesLista = new List<VMRol>();
                if (RolEditado != null)
                {
                    listRoles.Add(RolEditado);
                    foreach (var item in listRoles)
                    {
                        vmRolesLista.Add(new VMRol
                        {
                            IdRol=item.IdRol,
                            NombreRol=item.NombreRol,
                            Estado = Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmRolesLista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdRol)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _RolService.Eliminar(IdRol);

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

    }
}
