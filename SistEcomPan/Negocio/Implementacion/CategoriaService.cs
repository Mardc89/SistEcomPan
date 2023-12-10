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
    public class CategoriaService:ICategoriaService
    {
        private readonly IGenericRepository<Categorias> _repositorio;

        public CategoriaService(IGenericRepository<Categorias> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Categorias> Crear(Categorias entidad)
        {
            IQueryable<Categorias> categorias = await _repositorio.Consultar();
            IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.TipoDeCategoria == entidad.TipoDeCategoria);
            Categorias categoriaExiste = categoriaEvaluada.FirstOrDefault();


            if (categoriaExiste != null)
                throw new TaskCanceledException("La Categoria ya Existe");

            try
            {
                Categorias categoriaCreada = await _repositorio.Crear(entidad);

                if (categoriaCreada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear la Categoria");

                return categoriaCreada;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Categorias> Editar(Categorias entidad)
        {

            IQueryable<Categorias> categorias = await _repositorio.Consultar();
            IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.TipoDeCategoria == entidad.TipoDeCategoria && u.IdCategoria != entidad.IdCategoria);
            Categorias categoriaExiste = categoriaEvaluada.FirstOrDefault();


            if (categoriaExiste != null)
                throw new TaskCanceledException("La Categoria ya Existe");

            try
            {
                IQueryable<Categorias> buscarCategoria = await _repositorio.Consultar();
                IQueryable<Categorias> categoriaEncontrada = buscarCategoria.Where(u => u.IdCategoria == entidad.IdCategoria);
                Categorias categoriaEditar = categoriaEncontrada.First();

                categoriaEditar.TipoDeCategoria = entidad.TipoDeCategoria;
                categoriaEditar.Estado = entidad.Estado;

                bool respuesta = await _repositorio.Editar(categoriaEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar la Categoria");

                return categoriaEditar;


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
                IQueryable<Categorias> categorias = await _repositorio.Consultar();
                IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.IdCategoria == IdCategoria);
                Categorias categoriaEncontrada = categoriaEvaluada.FirstOrDefault();

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La Categoria no Existe");

                bool respuesta = await _repositorio.Eliminar(categoriaEncontrada.IdCategoria);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Categorias>> Lista()
        {
            List<Categorias> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Categorias>> ObtenerNombre()
        {
            List<Categorias> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }
    }
}
