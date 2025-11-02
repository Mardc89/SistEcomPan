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
        private readonly IRolRepository _repositorio;

        public RolService(IRolRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> ConsultarRol(int? IdRol)
        {
            Roles roles = await _repositorio.Buscar(null,null,IdRol);
            return roles.NombreRol;
        }

        public async Task<List<Roles>> Lista()
        {
            List<Roles> query = await _repositorio.Lista();
            return query;
        }

        public async Task<IQueryable<Roles>> ObtenerNombre()
        {
            List<Roles> lista = await _repositorio.Lista();
            return lista.AsQueryable();
        }

        public async Task<Roles> Crear(Roles entidad)
        {
            Roles rolExiste = await _repositorio.Buscar(entidad.NombreRol,null, null);

            if (rolExiste != null)
                throw new TaskCanceledException("El Rol ya Existe");

            try
            {
                Roles RolCreado = await _repositorio.Crear(entidad);

                if (RolCreado.IdRol == 0)
                    throw new TaskCanceledException("No se pudo crear el Rol");

                return RolCreado;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Roles> Editar(Roles entidad)
        {

            Roles rolExiste = await _repositorio.Verificar(entidad.NombreRol,null, entidad.IdRol);

            if (rolExiste != null)
                throw new TaskCanceledException("El Rol ya Existe");

            try
            {
                Roles rolEditar = await _repositorio.Buscar(null, null, entidad.IdRol);
                rolEditar.NombreRol = entidad.NombreRol;
                rolEditar.Estado = entidad.Estado;

                bool respuesta = await _repositorio.Editar(rolEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el rol");

                return rolEditar;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int IdRol)
        {
            try
            {
                Roles rolEncontrado = await _repositorio.Buscar(null, null, IdRol);
                if (rolEncontrado == null)
                    throw new TaskCanceledException("El Rol no Existe");

                bool respuesta = await _repositorio.Eliminar(rolEncontrado.IdRol);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
