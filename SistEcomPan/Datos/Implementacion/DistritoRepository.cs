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
    public class DistritoRepository:IGenericRepository<Distritos>
    {
        private readonly string _cadenaSQL = "";

        public DistritoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Distritos>> Lista()
        {
            List<Distritos> lista = new List<Distritos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDistritos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Distritos
                        {
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreDistrito = dr["NombreDistrito"].ToString()
                        }) ;
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Distritos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarDistritos", conexion);
                cmd.Parameters.AddWithValue("NombreDistrito", modelo.NombreDistrito);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Distritos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarDistritos", conexion);
                cmd.Parameters.AddWithValue("@IdDistrito", modelo.IdDistrito);
                cmd.Parameters.AddWithValue("@NombreDistrito", modelo.NombreDistrito);
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
                SqlCommand cmd = new SqlCommand("SPEliminarDistritos", conexion);
                cmd.Parameters.AddWithValue("@IdDistrito", id);                                                                                                                                                                                             
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<IQueryable<Distritos>> Consultar(Expression<Func<Distritos, bool>> filtro = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Distritos> Crear(Distritos modelo)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarDistrito", conexion);
                    cmd.Parameters.AddWithValue("@NombreDeDistrito", modelo.NombreDistrito);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdDistrito";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int DistritoId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdDistrito = DistritoId;

                    return modelo;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Task<IQueryable<Distritos>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Distritos>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Distritos> Buscar(string? Descripcion = null, string? NombreDistrito = null, int? IdDistrito = null)
        {
            Distritos lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDistritos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDistrito", (object)IdDistrito ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreDistrito", (object)NombreDistrito ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Distritos
                        {
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreDistrito = dr["NombreDistrito"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Distritos> Verificar(string? Descripcion = null, string? NombreDistrito = null, int? IdDistrito = null)
        {
            Distritos lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarNombreDistrito", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDistrito", (object)IdDistrito ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreDistrito", (object)NombreDistrito ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Distritos
                        {
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreDistrito = dr["NombreDistrito"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public Task<List<Distritos>> ConsultarLista()
        {
            throw new NotImplementedException();
        }

        public Task<List<Distritos>> Consultar(string? c = null, string? p = null, string? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
