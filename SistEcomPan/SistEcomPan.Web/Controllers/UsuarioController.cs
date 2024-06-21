using Microsoft.AspNetCore.Mvc;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using Negocio.Implementacion;
using SistEcomPan.Web.Models.ViewModels;
using Newtonsoft.Json;
using SistEcomPan.Web.Tools.Response;
using Microsoft.AspNetCore.Authorization;

namespace SistEcomPan.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolService;
        private readonly IEncriptService _encriptService;

        public UsuarioController(IUsuarioService usuarioServicio,IRolService rolService, IEncriptService encriptService)
        {
            _usuarioServicio = usuarioServicio;
            _rolService = rolService;
            _encriptService = encriptService;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolService.Lista();
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
            var nombreRol = await _rolService.ObtenerNombre();
            foreach (var item in Usuariolista)
            {
                vmUsuariolista.Add(new VMUsuario
                {
                    IdUsuario = item.IdUsuario,
                    Dni = item.Dni,
                    Nombres = item.Nombres,
                    Apellidos = item.Apellidos,
                    Correo = item.Correo,
                    NombreUsuario = item.NombreUsuario,
                    Clave = _encriptService.DesencriptarPassword(item.Clave),
                    IdRol = item.IdRol,
                    EsActivo = Convert.ToInt32(item.Estado),
                    NombreRol = nombreRol.Where(x=>x.IdRol==item.IdRol).First().NombreRol,
                    UrlFoto=item.UrlFoto,
                    NombreFoto=item.NombreFoto
                    
                }) ; 
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
                            Dni = item.Dni,
                            Nombres = item.Nombres,
                            Apellidos = item.Apellidos,
                            Correo = item.Correo,
                            NombreUsuario = item.NombreUsuario,
                            Clave = item.Clave,
                            IdRol = item.IdRol,
                            Estado = Convert.ToBoolean(item.EsActivo),
                            UrlFoto=item.UrlFoto
                            
                        }) ;
                    }
                }

                Usuarios usuarioCreado=await _usuarioServicio.Crear(listaUsuarios.First(),fotoStream,NombreFoto,urlPlantillaCorreo);

                List<VMUsuario> vmUsuariolista = new List<VMUsuario>();
                List<Usuarios> listUsuarios = new List<Usuarios>();
                var nombreRol = await _rolService.ObtenerNombre();
                if (usuarioCreado != null)
                {
                    listUsuarios.Add(usuarioCreado);

                   
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
                            Clave = _encriptService.DesencriptarPassword(item.Clave),
                            IdRol = item.IdRol,
                            NombreRol = nombreRol.Where(x => x.IdRol == item.IdRol).First().NombreRol,
                            UrlFoto=item.UrlFoto,
                            EsActivo=Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmUsuariolista.First();

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
                            Estado = Convert.ToBoolean(item.EsActivo),
                            UrlFoto=item.UrlFoto
                           

                        });
                    }
                }

                Usuarios usuarioEditado = await _usuarioServicio.Editar(listaUsuarios.First(), fotoStream, NombreFoto);

                List<Usuarios> listUsuarios = new List<Usuarios>(); 
                List<VMUsuario> vmUsuariolista = new List<VMUsuario>();
                if (usuarioEditado != null)
                {
                    listUsuarios.Add(usuarioEditado);

                   
                    var nombreRol = await _rolService.ObtenerNombre();
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
                            Clave = _encriptService.DesencriptarPassword(item.Clave),
                            IdRol = item.IdRol,
                            NombreRol = nombreRol.Where(x => x.IdRol == item.IdRol).First().NombreRol,
                            UrlFoto=item.UrlFoto,
                            EsActivo=Convert.ToInt32(item.Estado)
                            
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmUsuariolista.First();

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
