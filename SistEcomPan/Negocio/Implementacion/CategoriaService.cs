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

        public Task<Categorias> Crear(Categorias entidad)
        {
            IQueryable<Categorias> categorias = await _repositorio.Consultar();
            IQueryable<Categorias> categoriaEvaluada = categorias.Where(u => u.Descripcion == entidad.Descripcion);
            Productos productoExiste = categoriaEvaluada.FirstOrDefault();


            if (productoExiste != null)
                throw new TaskCanceledException("El Producto ya Existe");

            try
            {
                



                Categorias categoriaCreada = await _repositorio.Crear(entidad);

                if (categoriaCreada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear el Producto");

                return categoriaCreada;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Task<Categorias> Editar(Categorias entidad)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdCategoria)
        {
            throw new NotImplementedException();
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
