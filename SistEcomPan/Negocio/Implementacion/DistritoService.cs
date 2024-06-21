using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class DistritoService:IDistritoService
    {
        private readonly IGenericRepository<Distritos> _repositorio;

        public DistritoService(IGenericRepository<Distritos> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Distritos>> Lista()
        {
            List<Distritos> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Distritos>> ObtenerNombre()
        {
            List<Distritos> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }

        public async Task<string> ConsultarDistrito(int? IdDistrito)
        {
            Distritos distritos = await _repositorio.Buscar(null, null, IdDistrito);
            return distritos.NombreDistrito;
        }

        public async Task<Distritos> Crear(Distritos entidad)
        {
            //IQueryable<Categorias> categorias = await _repositorio.Consultar();
            //IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.TipoDeCategoria == entidad.TipoDeCategoria);
            //Categorias categoriaExiste = categoriaEvaluada.FirstOrDefault();
            Distritos distritoExiste = await _repositorio.Buscar(null, entidad.NombreDistrito, null);

            if (distritoExiste != null)
                throw new TaskCanceledException("La Categoria ya Existe");

            try
            {
                Distritos DistritoCreado = await _repositorio.Crear(entidad);

                if (DistritoCreado.IdDistrito == 0)
                    throw new TaskCanceledException("No se pudo crear la Categoria");

                return DistritoCreado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Distritos> Editar(Distritos entidad)
        {

            //IQueryable<Categorias> categorias = await _repositorio.Consultar();
            //IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.TipoDeCategoria == entidad.TipoDeCategoria && u.IdCategoria != entidad.IdCategoria);
            //Categorias categoriaExiste = categoriaEvaluada.FirstOrDefault();
            Distritos distritoExiste = await _repositorio.Verificar(null, entidad.NombreDistrito, entidad.IdDistrito);

            if (distritoExiste != null)
                throw new TaskCanceledException("La Categoria ya Existe");

            try
            {
                //IQueryable<Categorias> buscarCategoria = await _repositorio.Consultar();
                //IQueryable<Categorias> categoriaEncontrada = buscarCategoria.Where(u => u.IdCategoria == entidad.IdCategoria);
                //Categorias categoriaEditar = categoriaEncontrada.First();
                Distritos distritoEditar = await _repositorio.Buscar(null, null, entidad.IdDistrito);
                distritoEditar.NombreDistrito = entidad.NombreDistrito;

                bool respuesta = await _repositorio.Editar(distritoEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar la Categoria");

                return distritoEditar;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int IdCategoria)
        {
            try
            {
                //IQueryable<Categorias> categorias = await _repositorio.Consultar();
                //IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.IdCategoria == IdCategoria);
                //Categorias categoriaEncontrada = categoriaEvaluada.FirstOrDefault();
                Distritos distritoEncontrado = await _repositorio.Buscar(null, null, IdCategoria);
                if (distritoEncontrado == null)
                    throw new TaskCanceledException("La Categoria no Existe");

                bool respuesta = await _repositorio.Eliminar(distritoEncontrado.IdDistrito);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
