using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
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
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])

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
                cmd.Parameters.AddWithValue("@IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("@TipoDeCategoria", modelo.TipoDeCategoria);
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
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
                SqlCommand cmd = new SqlCommand("SPEliminarCategorias", conexion);
                cmd.Parameters.AddWithValue("@IdCategoria",id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public async Task<IQueryable<Categorias>> Obtener(string consulta)
        {
            List<Categorias> lista = new List<Categorias>();

            using (var connection = new SqlConnection(_cadenaSQL))
            {
                using (var command = new SqlCommand("BuscarCategoria", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Texto", SqlDbType.NVarChar, 100).Value =consulta;

                    try
                    {
                        connection.Open();
                        SqlDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            lista.Add(new Categorias
                            {
                                IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                                Estado = dr.GetBoolean(dr.GetOrdinal("Estado"))

                            });

                        }

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        // Manejo de excepciones
                        Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                    }
                }
            }

            return lista.AsQueryable();

        }

        public async Task<Categorias> Crear(Categorias modelo)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarCategoria", conexion);
                    cmd.Parameters.AddWithValue("@TipoDeCategoria", modelo.TipoDeCategoria);
                    cmd.Parameters.AddWithValue("@Estado", modelo.Estado);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdCategoria";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int CategoriaId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdCategoria = CategoriaId;

                    return modelo;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IQueryable<Categorias>> Consultar()
        {
            List<Categorias> lista = new List<Categorias>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaCategorias", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Categorias
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])                      
                        });
                    }
                }
            }

            return lista.AsQueryable();
        }

        public async Task<Categorias> Buscar(string? Estado = null, string?TipoDeCategoria = null, int? IdCategoria = null)
        {
            Categorias lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarCategorias", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCategoria", (object)IdCategoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoDeCategoria", (object)TipoDeCategoria ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Categorias
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return lista;

        }

        public async Task<Categorias> Verificar(string? Estado = null, string?TipoDeCategoria = null, int? IdCategoria = null)
        {
            Categorias lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarTipoDeCategoria", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCategoria", (object)IdCategoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoDeCategoria", (object)TipoDeCategoria ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Categorias
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return lista;
        }

        public Task<List<Categorias>> ConsultarLista()
        {
            throw new NotImplementedException();
        }

        public Task<List<Categorias>> Consultar(string? c = null, string? p = null, string? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
