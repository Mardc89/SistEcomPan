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
        public async Task<List<Distritos>> lista()
        {
            List<Distritos> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Distritos>> ObtenerNombre()
        {
            List<Distritos> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }
    }
}
