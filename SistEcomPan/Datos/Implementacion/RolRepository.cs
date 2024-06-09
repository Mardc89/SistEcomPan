using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Interfaces;
using System.Linq.Expressions;

namespace Datos.Implementacion
{
    public class RolRepository:IGenericRepository<Roles>
    {
        private readonly string _cadenaSQL = "";

        public RolRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Roles>> Lista()
        {
            List<Roles> lista = new List<Roles>();
            try
            {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaRoles", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Roles
                        {
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            NombreRol = dr["NombreRol"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])

                        });
                    }
                }
            }
            }
            catch (Exception)
            {

                throw;
            }


            return lista;
        }

        public async Task<bool> Guardar(Roles modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarRoles", conexion);
                cmd.Parameters.AddWithValue("NombreRol", modelo.NombreRol);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Roles modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarRoles", conexion);
                cmd.Parameters.AddWithValue("IdRol", modelo.IdRol);
                cmd.Parameters.AddWithValue("NombreRol", modelo.NombreRol);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }

        }

        public async Task<bool> Eliminar(int id)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEliminarRoles", conexion);
                cmd.Parameters.AddWithValue("IdRol", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<IQueryable<Roles>> Consultar(Expression<Func<Roles, bool>> filtro = null)
        {
            throw new NotImplementedException();
        }

        public Task<Roles> Obtener(Roles modelo)
        {
            throw new NotImplementedException();
        }

        public Task<Roles> Crear(Roles modelo)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Roles>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Roles>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Roles> Buscar(string? NombreRol = null, string? Estado = null, int? IdRol = null)
        {
            Roles lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarRoles", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", (object)IdRol ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Roles
                        {

                            NombreRol = dr["NombreRol"].ToString(),

                        };
                    }
                }
            }

            return lista;
        }

        public Task<Roles> Verificar(string? NombreRol = null, string? Estado = null, int? IdRol = null)
        {
            throw new NotImplementedException();
        }
    }
}
