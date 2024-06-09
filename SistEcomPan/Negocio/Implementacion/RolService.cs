using Datos.Implementacion;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

        public async Task<string> ConsultarRol(int? IdRol)
        {
            Roles roles = await _repositorio.Buscar(null,null,IdRol);
            return roles.NombreRol;
        }

        public async Task<List<Roles>> lista()
        {
            List<Roles> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Roles>> ObtenerNombre()
        {
            List<Roles> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }
    }
}
