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
    [Authorize(Roles="Administrador")]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductoController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaCategorias()
        {
            var lista = await _categoriaService.Lista();
            List<VMCategoria> vmListaCategorias = new List<VMCategoria>();
            foreach (var item in lista)
            {
                vmListaCategorias.Add(new VMCategoria
                {
                    IdCategoria = item.IdCategoria,
                    TipoDeCategoria = item.TipoDeCategoria,
                    Estado=Convert.ToInt32(item.Estado)
                });
            }
            return StatusCode(StatusCodes.Status200OK, vmListaCategorias);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var Productolista = await _productoService.Lista();
            List<VMProducto> vmProductolista = new List<VMProducto>();
            var nombreCategoria = await _categoriaService.ObtenerNombre();
            foreach (var item in Productolista)
            {
                vmProductolista.Add(new VMProducto
                {
                    IdProducto = item.IdProducto,
                    Descripcion = item.Descripcion,
                    IdCategoria = item.IdCategoria,
                    Precio=item.Precio,
                    Estado=Convert.ToInt32(item.Estado),
                    Stock=item.Stock,
                    NombreCategoria = nombreCategoria.Where(x => x.IdCategoria == item.IdCategoria).First().TipoDeCategoria,
                    UrlImagen=item.UrlImagen,

                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmProductolista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();

                }
             
                List<Productos> listaProductos = new List<Productos>();
                List<VMProducto> listaVMProductos = new List<VMProducto>();
                if (vmProducto != null)
                {
                    listaVMProductos.Add(vmProducto);
                    foreach (var item in listaVMProductos)
                    {
                        listaProductos.Add(new Productos
                        {
                            IdProducto = item.IdProducto,
                            Descripcion = item.Descripcion,
                            IdCategoria = item.IdCategoria,
                            Precio=item.Precio,
                            Estado =Convert.ToBoolean(item.Estado),
                            Stock=item.Stock,
                            UrlImagen=item.UrlImagen
                            
                        });
                    }
                }

                Productos productoCreado = await _productoService.Crear(listaProductos.First(), fotoStream, NombreFoto);

                List<VMProducto> vmProductolista = new List<VMProducto>();
                List<Productos> listProductos = new List<Productos>();
                var nombreCategoria = await _categoriaService.ObtenerNombre();
                if (productoCreado != null)
                {
                    listProductos.Add(productoCreado);


                    foreach (var item in listProductos)
                    {
                        vmProductolista.Add(new VMProducto
                        {
                            IdProducto = item.IdProducto,
                            Descripcion = item.Descripcion,
                            IdCategoria = item.IdCategoria,
                            Precio=item.Precio,
                            Estado = Convert.ToInt32(item.Estado),
                            Stock=item.Stock,
                            UrlImagen=item.UrlImagen
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmProductolista.First();

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
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                string NombreFoto = "";
                Stream fotoStream = null;

                if (foto != null && foto.Length > 0)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    NombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();

                }

                List<Productos> listaProductos = new List<Productos>();
                List<VMProducto> listaVMProductos = new List<VMProducto>();
                if (vmProducto != null)
                {
                    listaVMProductos.Add(vmProducto);
                    foreach (var item in listaVMProductos)
                    {
                        listaProductos.Add(new Productos
                        {
                            IdProducto = item.IdProducto,
                            Descripcion = item.Descripcion,
                            IdCategoria = item.IdCategoria,
                            Precio=item.Precio,
                            Estado = Convert.ToBoolean(item.Estado),
                            Stock=item.Stock,
                            UrlImagen=item.UrlImagen
                            
                        });
                    }
                }

                Productos productoEditado = await _productoService.Editar(listaProductos.First(), fotoStream, NombreFoto);

                List<Productos> listProductos = new List<Productos>();
                List<VMProducto> vmProductolista = new List<VMProducto>();
                if (productoEditado != null)
                {
                    listProductos.Add(productoEditado);
                    var nombreCategoria = await _categoriaService.ObtenerNombre();
                    foreach (var item in listProductos)
                    {
                        vmProductolista.Add(new VMProducto
                        {
                            IdProducto = item.IdProducto,
                            Descripcion = item.Descripcion,
                            IdCategoria = item.IdCategoria,
                            Precio=item.Precio,
                            NombreCategoria = nombreCategoria.Where(x => x.IdCategoria == item.IdCategoria).First().TipoDeCategoria,
                            Estado=Convert.ToInt32(item.Estado),
                            Stock=item.Stock,
                            UrlImagen=item.UrlImagen

                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmProductolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdProducto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _productoService.Eliminar(IdProducto);

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