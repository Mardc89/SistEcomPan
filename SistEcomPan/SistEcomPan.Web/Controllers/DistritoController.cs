using Entidades;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;
using SistEcomPan.Web.Models.ViewModels;
using SistEcomPan.Web.Tools.Response;

namespace SistEcomPan.Web.Controllers
{
    public class DistritoController : Controller
    {
        private readonly IDistritoService _distritoService;

        public DistritoController(IDistritoService distritoService)
        {
            _distritoService = distritoService;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaDistritos()
        {
            var lista = await _distritoService.Lista();
            List<VMDistrito> vmListaDistritos = new List<VMDistrito>();
            foreach (var item in lista)
            {
                vmListaDistritos.Add(new VMDistrito
                {
                    IdDistrito = item.IdDistrito,
                    NombreDistrito= item.NombreDistrito
 
                });
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaDistritos });
        }

        //[HttpGet]
        //public async Task<IActionResult> Lista()
        //{
        //    var Categorialista = await _distritoService.Lista();
        //    List<VMCategoria> vmCategorialista = new List<VMCategoria>();
        //    foreach (var item in Categorialista)
        //    {
        //        vmCategorialista.Add(new VMCategoria
        //        {
        //            IdCategoria = item.IdCategoria,
        //            TipoDeCategoria = item.TipoDeCategoria,
        //            Estado = Convert.ToInt32(item.Estado)
        //        });
        //    }
        //    return StatusCode(StatusCodes.Status200OK, new { data = vmCategorialista });
        //}

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMDistrito modelo)
        {
            GenericResponse<VMDistrito> gResponse = new GenericResponse<VMDistrito>();

            try
            {
                List<Distritos> listaDistritos = new List<Distritos>();
                List<VMDistrito> listaVMDistritos = new List<VMDistrito>();
                if (modelo != null)
                {
                    listaVMDistritos.Add(modelo);
                    foreach (var item in listaVMDistritos)
                    {
                        listaDistritos.Add(new Distritos
                        {
                            IdDistrito=item.IdDistrito,
                            NombreDistrito=item.NombreDistrito
                        });
                    }
                }

                Distritos distritoCreado = await _distritoService.Crear(listaDistritos.First());

                List<VMDistrito> vmDistritolista = new List<VMDistrito>();
                List<Distritos> listCategorias = new List<Distritos>();
                if (distritoCreado != null)
                {
                    listCategorias.Add(distritoCreado);


                    foreach (var item in listCategorias)
                    {
                        vmDistritolista.Add(new VMDistrito
                        {
                            IdDistrito=item.IdDistrito,
                            NombreDistrito=item.NombreDistrito

                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmDistritolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMDistrito modelo)
        {
            GenericResponse<VMDistrito> gResponse = new GenericResponse<VMDistrito>();

            try
            {

                List<Distritos> listaDistritos = new List<Distritos>();
                List<VMDistrito> listaVMDistritos = new List<VMDistrito>();
                if (modelo != null)
                {
                    listaVMDistritos.Add(modelo);
                    foreach (var item in listaVMDistritos)
                    {
                        listaDistritos.Add(new Distritos
                        {
                            IdDistrito=item.IdDistrito,
                            NombreDistrito=item.NombreDistrito

                        });
                    }
                }

                Distritos distritoEditado = await _distritoService.Editar(listaDistritos.First());

                List<Distritos> listDistritos = new List<Distritos>();
                List<VMDistrito> vmDistritolista = new List<VMDistrito>();
                if (distritoEditado != null)
                {
                    listDistritos.Add(distritoEditado);
                    foreach (var item in listDistritos)
                    {
                        vmDistritolista.Add(new VMDistrito
                        {
                           IdDistrito=item.IdDistrito,
                           NombreDistrito=item.NombreDistrito
                        });
                    }
                }

                gResponse.Estado = true;
                gResponse.objeto = vmDistritolista.First();

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdDistrito)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _distritoService.Eliminar(IdDistrito);

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
