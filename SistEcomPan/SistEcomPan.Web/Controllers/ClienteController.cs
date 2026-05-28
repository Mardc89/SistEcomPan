
using Datos.Interfaces;
using Entidades;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Implementacion;
using Negocio.Interfaces;
using Newtonsoft.Json;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IDistritoService _distritoService;
        private readonly IEncriptService _encriptService;
        private readonly IMapper _mapper;

        public ClienteController(IClienteService clienteService, IDistritoService distritoService, IEncriptService encriptService,IMapper mapper)
        {
            _clienteService = clienteService;
            _distritoService = distritoService;
            _encriptService = encriptService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {

            return View();
        }


        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ListaDistritos()
        //{
        //    var lista = await _distritoService.Lista();
        //    List<VMDistrito> vmListaDistritos = new List<VMDistrito>();
        //    foreach (var item in lista)
        //    {
        //        vmListaDistritos.Add(new VMDistrito
        //        {
        //            IdDistrito = item.IdDistrito,
        //            NombreDistrito = item.NombreDistrito
        //        });
        //    }
        //    return StatusCode(StatusCodes.Status200OK, vmListaDistritos);
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListaDistritos()
        {
            var lista = await _distritoService.Lista();

            var vmLista = _mapper.Map<List<VMDistrito>>(lista);

            return Ok(vmLista);
        }


        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var clientes = await _clienteService.Lista();

            var vmLista = _mapper.Map<List<VMCliente>>(clientes);

            foreach (var item in vmLista) 
            {
                item.Clave = _encriptService.DesencriptarPassword(item.Clave);
            
            }

            return Ok(new { data = vmLista });
        }

        //[HttpGet]
        //public async Task<IActionResult> Lista()
        //{
        //    var clienteLista = await _clienteService.Lista();
        //    List<VMCliente> vmClienteLista = new List<VMCliente>();
        //    //var nombreDistrito = await _distritoService.ObtenerNombre();
        //    foreach (var item in clienteLista)
        //    {
        //        vmClienteLista.Add(new VMCliente
        //        {
        //            IdCliente = item.IdCliente,
        //            TipoCliente = item.TipoCliente,
        //            Dni = item.Dni,
        //            Nombres = item.Nombres,
        //            Apellidos = item.Apellidos,
        //            NombreCompleto = _clienteService.LimpiarEspacios(item.Apellidos + " " + item.Nombres),
        //            Correo = item.Correo,
        //            Direccion = item.Direccion,
        //            Telefono = item.Telefono,
        //            IdDistrito = item.IdDistrito,
        //            NombreUsuario = item.NombreUsuario,
        //            Clave = _encriptService.DesencriptarPassword(item.Clave),
        //            Estado = Convert.ToInt32(item.Estado),
        //            UrlFoto = item.UrlFoto,
        //            NombreFoto = item.NombreFoto,
        //            //NombreDistrito = nombreDistrito.Where(x => x.IdDistrito == item.IdDistrito).First().NombreDistrito,
        //            NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito)

        //        });
        //    }
        //    return StatusCode(StatusCodes.Status200OK, new { data = vmClienteLista });
        //}

        [HttpGet]
        public async Task<IActionResult> ObtenerClientes(string searchTerm = "", int page = 1, int itemsPerPage = 4)
        {
            var clientes = await _clienteService.ClienteFiltrado(searchTerm);

            var vmLista = _mapper.Map<List<VMCliente>>(clientes);

            var paginados = vmLista
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return Ok(new
            {
                clientes = paginados,
                totalItems = vmLista.Count
            });
        }


        // [HttpGet]
        // public async Task<IActionResult> ObtenerClientes(string searchTerm = "", int page = 1, int itemsPerPage = 4)
        //{
        //     //var clienteLista = await _clienteService.Lista();                       
        //     var pedidosFiltrados = await _clienteService.ClienteFiltrado(searchTerm);

        //     List<VMCliente> vmClienteLista = new List<VMCliente>();
        //     //var nombreDistrito = await _distritoService.ObtenerNombre();
        //     foreach (var item in pedidosFiltrados)
        //     {
        //         vmClienteLista.Add(new VMCliente
        //         {
        //             IdCliente = item.IdCliente,
        //             TipoCliente = item.TipoCliente,
        //             Dni = item.Dni,
        //             NombreCompleto = _clienteService.LimpiarEspacios(item.Apellidos + " " + item.Nombres),
        //             Correo = item.Correo,
        //             Direccion = item.Direccion,
        //             Telefono = item.Telefono,
        //             NombreUsuario = item.NombreUsuario,
        //             //NombreFoto = item.NombreFoto,
        //             NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito)

        //         });
        //     }

        //     var pedidosPaginados = vmClienteLista.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

        //     return StatusCode(StatusCodes.Status200OK, new { clientes = pedidosPaginados, totalItems = vmClienteLista.Count() });
        // }


        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            var response = new GenericResponse<VMCliente>();

            try
            {
                var vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);

                var cliente = _mapper.Map<Clientes>(vmCliente);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    nombreFoto = $"{Guid.NewGuid():N}{Path.GetExtension(foto.FileName)}";
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{Request.Scheme}://{Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                var creado = await _clienteService.Crear(cliente, fotoStream, nombreFoto, urlPlantillaCorreo);

                var vm = _mapper.Map<VMCliente>(creado);

                // solo lo que NO hace Mapster
                vm.Clave = _encriptService.DesencriptarPassword(creado.Clave);

                response.Estado = true;
                response.objeto = vm;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return Ok(response);
        }

        //[HttpPost]
        //public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        //{
        //    GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();

        //    try
        //    {
        //        VMCliente vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);
        //        string NombreFoto = "";
        //        Stream fotoStream = null;

        //        if (foto != null && foto.Length > 0)
        //        {
        //            string nombreCodigo = Guid.NewGuid().ToString("N");
        //            string extension = Path.GetExtension(foto.FileName);
        //            NombreFoto = string.Concat(nombreCodigo, extension);
        //            fotoStream = foto.OpenReadStream();

        //        }
        //        string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
        //        var Clientelista = await _clienteService.Lista();


        //        List<Clientes> listaClientes = new List<Clientes>();
        //        List<VMCliente> listaVMClientes = new List<VMCliente>();
        //        if (vmCliente != null)
        //        {
        //            listaVMClientes.Add(vmCliente);
        //            foreach (var item in listaVMClientes)
        //            {
        //                listaClientes.Add(new Clientes
        //                {
        //                    IdCliente = item.IdCliente,
        //                    TipoCliente=item.TipoCliente,
        //                    Dni = item.Dni,
        //                    Nombres = item.Nombres,
        //                    Apellidos = item.Apellidos,
        //                    Correo = item.Correo,
        //                    Direccion=item.Direccion,
        //                    Telefono=item.Telefono,  
        //                    IdDistrito = item.IdDistrito,
        //                    NombreUsuario = item.NombreUsuario,
        //                    Clave = item.Clave,                         
        //                    Estado = Convert.ToBoolean(item.Estado),
        //                    UrlFoto = item.UrlFoto

        //                });
        //            }
        //        }

        //        Clientes usuarioCreado = await _clienteService.Crear(listaClientes.First(), fotoStream, NombreFoto, urlPlantillaCorreo);

        //        List<VMCliente> vmClientelista = new List<VMCliente>();
        //        List<Clientes> listClientes = new List<Clientes>();
        //        //var nombreDistrito = await _distritoService.ObtenerNombre();
        //        if (usuarioCreado != null)
        //        {
        //            listClientes.Add(usuarioCreado);


        //            foreach (var item in listClientes)
        //            {
        //                vmClientelista.Add(new VMCliente
        //                {
        //                    IdCliente = item.IdCliente,
        //                    TipoCliente=item.TipoCliente,
        //                    Dni = item.Dni,
        //                    Nombres = item.Nombres,
        //                    Apellidos = item.Apellidos,
        //                    Correo = item.Correo,
        //                    Direccion=item.Direccion,
        //                    Telefono=item.Telefono, 
        //                    IdDistrito = item.IdDistrito,
        //                    NombreUsuario = item.NombreUsuario,
        //                    Clave = _encriptService.DesencriptarPassword(item.Clave),  
        //                    Estado = Convert.ToInt32(item.Estado),                          
        //                    NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito),
        //                    UrlFoto = item.UrlFoto,

        //                });
        //            }
        //        }

        //        gResponse.Estado = true;
        //        gResponse.objeto = vmClientelista.First();

        //    }
        //    catch (Exception ex)
        //    {
        //        gResponse.Estado = false;
        //        gResponse.Mensaje = ex.Message;

        //    }

        //    return StatusCode(StatusCodes.Status200OK, gResponse);

        //}

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            var response = new GenericResponse<VMCliente>();

            try
            {
                var vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);

                var cliente = _mapper.Map<Clientes>(vmCliente);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    nombreFoto = $"{Guid.NewGuid():N}{Path.GetExtension(foto.FileName)}";
                    fotoStream = foto.OpenReadStream();
                }

                var editado = await _clienteService.Editar(cliente, fotoStream, nombreFoto);

                var vm = editado.Adapt<VMCliente>();

                vm.Clave = _encriptService.DesencriptarPassword(editado.Clave);

                response.Estado = true;
                response.objeto = vm;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return Ok(response);
        }

        //[HttpPut]
        //public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        //{
        //    GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();

        //    try
        //    {
        //        VMCliente vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);
        //        string NombreFoto = "";
        //        Stream fotoStream = null;

        //        if (foto != null && foto.Length > 0)
        //        {
        //            string nombreCodigo = Guid.NewGuid().ToString("N");
        //            string extension = Path.GetExtension(foto.FileName);
        //            NombreFoto = string.Concat(nombreCodigo, extension);
        //            fotoStream = foto.OpenReadStream();

        //        }

        //        var Clientelista = await _clienteService.Lista();


        //        List<Clientes> listaClientes = new List<Clientes>();
        //        List<VMCliente> listaVMClientes = new List<VMCliente>();
        //        if (vmCliente != null)
        //        {
        //            listaVMClientes.Add(vmCliente);
        //            foreach (var item in listaVMClientes)
        //            {
        //                listaClientes.Add(new Clientes
        //                {
        //                    IdCliente = item.IdCliente,
        //                    TipoCliente=item.TipoCliente,
        //                    Dni = item.Dni,
        //                    Nombres = item.Nombres,
        //                    Apellidos = item.Apellidos,
        //                    Correo = item.Correo,
        //                    Direccion=item.Direccion,
        //                    Telefono=item.Telefono,   
        //                    IdDistrito = item.IdDistrito,
        //                    NombreUsuario = item.NombreUsuario,
        //                    Clave = item.Clave,                        
        //                    Estado = Convert.ToBoolean(item.Estado),
        //                    UrlFoto = item.UrlFoto

        //                });
        //            }
        //        }

        //        Clientes clienteEditado = await _clienteService.Editar(listaClientes.First(), fotoStream, NombreFoto);

        //        List<Clientes> listClientes = new List<Clientes>();
        //        List<VMCliente> vmClientelista = new List<VMCliente>();
        //        if (clienteEditado != null)
        //        {
        //            listClientes.Add(clienteEditado);


        //            //var nombreDistrito = await _distritoService.ObtenerNombre();
        //            foreach (var item in listClientes)
        //            {
        //                vmClientelista.Add(new VMCliente
        //                {
        //                    IdCliente = item.IdCliente,
        //                    TipoCliente=item.TipoCliente,
        //                    Dni = item.Dni,
        //                    Nombres = item.Nombres,
        //                    Apellidos = item.Apellidos,
        //                    Correo = item.Correo,
        //                    Direccion=item.Direccion,
        //                    Telefono=item.Telefono,   
        //                    IdDistrito = item.IdDistrito,
        //                    NombreUsuario = item.NombreUsuario,
        //                    Clave = _encriptService.DesencriptarPassword(item.Clave),
        //                    Estado = Convert.ToInt32(item.Estado),
        //                    NombreDistrito = await _distritoService.ConsultarDistrito(item.IdDistrito),
        //                    UrlFoto = item.UrlFoto,


        //                });
        //            }
        //        }

        //        gResponse.Estado = true;
        //        gResponse.objeto = vmClientelista.First();

        //    }
        //    catch (Exception ex)
        //    {
        //        gResponse.Estado = false;
        //        gResponse.Mensaje = ex.Message;

        //    }

        //    return StatusCode(StatusCodes.Status200OK, gResponse);

        //}

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
