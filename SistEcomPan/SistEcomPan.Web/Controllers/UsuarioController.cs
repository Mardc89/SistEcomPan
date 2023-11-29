using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using Negocio.Implementacion;
using SistEcomPan.Web.Models.ViewModels;
using Newtonsoft.Json;
using SistEcomPan.Web.Tools.Response;

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
                    Dni=item.Dni,
                    Nombres=item.Nombres,
                    Apellidos=item.Apellidos,
                    Correo=item.Correo,
                    NombreUsuario=item.NombreUsuario,
                    Clave=item.Clave,
                    IdRol=item.IdRol,
                    //NombreRol = item.Rol.NombreRol
                });
            }
            return StatusCode(StatusCodes.Status200OK,new { data = vmUsuariolista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm]IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

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


                List<Usuarios>  listaUsuarios = new List<Usuarios>();
                List<VMUsuario> listaVMUsuarios = new List<VMUsuario>();
                if (vmUsuario != null)
                {
                    listaVMUsuarios.Add(vmUsuario);
                    foreach (var item in listaVMUsuarios)
                    {
                        listaUsuarios.Add(new Usuarios
                        {
                            IdUsuario = item.IdUsuario,
                            Dni=item.Dni,
                            Nombres = item.Nombres,
                            Apellidos=item.Apellidos,
                            Correo=item.Correo,
                            NombreUsuario=item.NombreUsuario,
                            Clave=item.Clave,
                            IdRol=item.IdRol,
                            Estado = Convert.ToBoolean(item.EsActivo)

                        });
                    }
                }

                Usuarios usuarioCreado=await _usuarioServicio.Crear(listaUsuarios.First(),fotoStream,NombreFoto,urlPlantillaCorreo);


                List<Usuarios> listUsuarios = new List<Usuarios>();
                if (usuarioCreado != null)
                {
                    listUsuarios.Add(usuarioCreado);

                    List<VMUsuario> vmUsuariolista = new List<VMUsuario>();
                    foreach (var item in listUsuarios)
                    {
                        vmUsuariolista.Add(new VMUsuario
                        {
                            IdUsuario = item.IdUsuario,
                            Dni = item.Dni,
                            Nombres = item.Nombres,
                            Apellidos = item.Apellidos,
                            Correo = item.Correo,
                            NombreUsuario = item.NombreUsuario,
                            Clave = item.Clave,
                            IdRol = item.IdRol,
                            NombreRol = item.Rol.NombreRol
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmUsuario;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                
            }

            return StatusCode(StatusCodes.Status200OK,gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

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

                var Usuariolista = await _usuarioServicio.Lista();


                List<Usuarios> listaUsuarios = new List<Usuarios>();
                List<VMUsuario> listaVMUsuarios = new List<VMUsuario>();
                if (vmUsuario != null)
                {
                    listaVMUsuarios.Add(vmUsuario);
                    foreach (var item in listaVMUsuarios)
                    {
                        listaUsuarios.Add(new Usuarios
                        {
                            IdUsuario = item.IdUsuario,
                            Dni = item.Dni,
                            Nombres = item.Nombres,
                            Apellidos = item.Apellidos,
                            Correo = item.Correo,
                            NombreUsuario = item.NombreUsuario,
                            Clave = item.Clave,
                            IdRol = item.IdRol,
                            Estado = Convert.ToBoolean(item.EsActivo)

                        });
                    }
                }

                Usuarios usuarioEditado = await _usuarioServicio.Editar(listaUsuarios.First(), fotoStream, NombreFoto);

                List<Usuarios> listUsuarios = new List<Usuarios>();
                if (usuarioEditado != null)
                {
                    listUsuarios.Add(usuarioEditado);

                    List<VMUsuario> vmUsuariolista = new List<VMUsuario>();
                    foreach (var item in listUsuarios)
                    {
                        vmUsuariolista.Add(new VMUsuario
                        {
                            IdUsuario = item.IdUsuario,
                            Dni = item.Dni,
                            Nombres = item.Nombres,
                            Apellidos = item.Apellidos,
                            Correo = item.Correo,
                            NombreUsuario = item.NombreUsuario,
                            Clave = item.Clave,
                            IdRol = item.IdRol,
                            NombreRol = item.Rol.NombreRol
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmUsuario;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario);

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
