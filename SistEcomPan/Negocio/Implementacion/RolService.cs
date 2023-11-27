using Datos.Implementacion;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Roles> _repositorio;

        public RolService(IGenericRepository<Roles> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Roles>> lista()
        {
            List<Roles> query = await _repositorio.Lista();
            return query;
        }



    }
}
