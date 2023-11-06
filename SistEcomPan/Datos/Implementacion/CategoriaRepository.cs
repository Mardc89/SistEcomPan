using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class CategoriaRepository : IGenericRepository<Categorias>
    {
        private readonly string _cadenaSQL = "";

        public CategoriaRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Categorias>> Lista()
        {
            List<Categorias> lista= new List<Categorias>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaCategorias",conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using(var dr= await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Categorias
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                            FechaRegistro = dr.GetDateTime(dr.GetOrdinal("FechaRegistro")),
                            Estado= dr.GetBoolean(dr.GetOrdinal("Estado"))

                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Categorias modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarCategorias", conexion);
                cmd.Parameters.AddWithValue("TipoDeCategoria", modelo.TipoDeCategoria);
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada> 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Categorias modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarCategorias", conexion);
                cmd.Parameters.AddWithValue("IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("TipoDeCategoria", modelo.TipoDeCategoria);
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }

        }

        public async Task<bool> Delete(int id)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEliminarCategorias", conexion);
                cmd.Parameters.AddWithValue("IdCategoria",id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }



    }
}
