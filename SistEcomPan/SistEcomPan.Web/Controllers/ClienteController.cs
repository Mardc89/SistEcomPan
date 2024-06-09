
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
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IDistritoService _distritoService;
        private readonly IEncriptService _encriptService;

        public ClienteController(IClienteService clienteService, IDistritoService distritoService, IEncriptService encriptService)
        {
            _clienteService = clienteService;
            _distritoService = distritoService;
            _encriptService = encriptService;
        }
        public IActionResult Index()
        {

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListaDistritos()
        {
            var lista = await _distritoService.lista();
            List<VMDistrito> vmListaDistritos = new List<VMDistrito>();
            foreach (var item in lista)
            {
                vmListaDistritos.Add(new VMDistrito
                {
                    IdDistrito = item.IdDistrito,
                    NombreDistrito = item.NombreDistrito
                });
            }
            return StatusCode(StatusCodes.Status200OK, vmListaDistritos);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var clienteLista = await _clienteService.Lista();
            List<VMCliente> vmClienteLista = new List<VMCliente>();
            var nombreDistrito = await _distritoService.ObtenerNombre();
            foreach (var item in clienteLista)
            {
                vmClienteLista.Add(new VMCliente
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
                    Clave = _encriptService.DesencriptarPassword(item.Clave), 
                    Estado = Convert.ToInt32(item.Estado),
                    UrlFoto = item.UrlFoto,                 
                    //NombreDistrito = nombreDistrito.Where(x => x.IdDistrito == item.IdDistrito).First().NombreDistrito,
                    NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito)

                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmClienteLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();

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
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                var Clientelista = await _clienteService.Lista();


                List<Clientes> listaClientes = new List<Clientes>();
                List<VMCliente> listaVMClientes = new List<VMCliente>();
                if (vmCliente != null)
                {
                    listaVMClientes.Add(vmCliente);
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
                            Clave = item.Clave,                         
                            Estado = Convert.ToBoolean(item.Estado),
                            UrlFoto = item.UrlFoto

                        });
                    }
                }

                Clientes usuarioCreado = await _clienteService.Crear(listaClientes.First(), fotoStream, NombreFoto, urlPlantillaCorreo);

                List<VMCliente> vmClientelista = new List<VMCliente>();
                List<Clientes> listClientes = new List<Clientes>();
                var nombreDistrito = await _distritoService.ObtenerNombre();
                if (usuarioCreado != null)
                {
                    listClientes.Add(usuarioCreado);


                    foreach (var item in listClientes)
                    {
                        vmClientelista.Add(new VMCliente
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
                            Clave = _encriptService.DesencriptarPassword(item.Clave),  
                            Estado = Convert.ToInt32(item.Estado),                          
                            NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito),
                            UrlFoto = item.UrlFoto,
                          
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmClientelista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();

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

                var Clientelista = await _clienteService.Lista();


                List<Clientes> listaClientes = new List<Clientes>();
                List<VMCliente> listaVMClientes = new List<VMCliente>();
                if (vmCliente != null)
                {
                    listaVMClientes.Add(vmCliente);
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
                            Clave = item.Clave,                        
                            Estado = Convert.ToBoolean(item.Estado),
                            UrlFoto = item.UrlFoto

                        });
                    }
                }

                Clientes clienteEditado = await _clienteService.Editar(listaClientes.First(), fotoStream, NombreFoto);

                List<Clientes> listClientes = new List<Clientes>();
                List<VMCliente> vmClientelista = new List<VMCliente>();
                if (clienteEditado != null)
                {
                    listClientes.Add(clienteEditado);


                    var nombreDistrito = await _distritoService.ObtenerNombre();
                    foreach (var item in listClientes)
                    {
                        vmClientelista.Add(new VMCliente
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
                            Clave = _encriptService.DesencriptarPassword(item.Clave),
                            Estado = Convert.ToInt32(item.Estado),
                            NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito),
                            UrlFoto = item.UrlFoto,
                          

                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmClientelista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdCliente)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _clienteService.Eliminar(IdCliente);

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
