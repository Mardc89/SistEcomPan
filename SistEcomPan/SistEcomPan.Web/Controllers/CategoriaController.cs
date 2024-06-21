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
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
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
                    Estado = Convert.ToInt32(item.Estado)
                });
            }
            return StatusCode(StatusCodes.Status200OK, vmListaCategorias);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var Categorialista = await _categoriaService.Lista();
            List<VMCategoria> vmCategorialista = new List<VMCategoria>();
            foreach (var item in Categorialista)
            {
                vmCategorialista.Add(new VMCategoria
                {
                    IdCategoria=item.IdCategoria,
                    TipoDeCategoria = item.TipoDeCategoria,
                    Estado = Convert.ToInt32(item.Estado)
                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmCategorialista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                List<Categorias> listaCategorias = new List<Categorias>();
                List<VMCategoria> listaVMCategorias = new List<VMCategoria>();
                if (modelo != null)
                {
                    listaVMCategorias.Add(modelo);
                    foreach (var item in listaVMCategorias)
                    {
                        listaCategorias.Add(new Categorias
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria=item.TipoDeCategoria,
                            Estado = Convert.ToBoolean(item.Estado)

                        });
                    }
                }

                Categorias categoriaCreada = await _categoriaService.Crear(listaCategorias.First());

                List<VMCategoria> vmCategorialista = new List<VMCategoria>();
                List<Categorias> listCategorias = new List<Categorias>();
                if (categoriaCreada != null)
                {
                    listCategorias.Add(categoriaCreada);


                    foreach (var item in listCategorias)
                    {
                        vmCategorialista.Add(new VMCategoria
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria=item.TipoDeCategoria,
                            Estado = Convert.ToInt32(item.Estado)
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmCategorialista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {

                List<Categorias> listaCategorias = new List<Categorias>();
                List<VMCategoria> listaVMCategorias = new List<VMCategoria>();
                if (modelo != null)
                {
                    listaVMCategorias.Add(modelo);
                    foreach (var item in listaVMCategorias)
                    {
                        listaCategorias.Add(new Categorias
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria=item.TipoDeCategoria,
                            Estado = Convert.ToBoolean(item.Estado)
                        });
                    }
                }

                Categorias categoriaEditada = await _categoriaService.Editar(listaCategorias.First());

                List<Categorias> listCategorias = new List<Categorias>();
                List<VMCategoria> vmCategorialista = new List<VMCategoria>();
                if (categoriaEditada != null)
                {
                    listCategorias.Add(categoriaEditada);
                    foreach (var item in listCategorias)
                    {
                        vmCategorialista.Add(new VMCategoria
                        {
                            IdCategoria = item.IdCategoria,
                            TipoDeCategoria=item.TipoDeCategoria,
                            Estado = Convert.ToInt32(item.Estado)                         
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmCategorialista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdCategoria)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _categoriaService.Eliminar(IdCategoria);

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