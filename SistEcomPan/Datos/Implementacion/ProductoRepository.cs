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
    public class ProductoRepository:IProductoNew
    {
        private readonly string _cadenaSQL = "";

        public ProductoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Productos>> Lista()
        {
            List<Productos> lista = new List<Productos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaProductos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Productos
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            UrlImagen = dr["UrlImagen"].ToString(), 
                            NombreImagen= dr["NombreImagen"].ToString(), 
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<Productos> Crear(Productos modelo)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPRegistrarProductos", conexion);
                cmd.Parameters.AddWithValue("@Descripcion", modelo.Descripcion);
                cmd.Parameters.AddWithValue("@IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("@Precio", modelo.Precio);
                cmd.Parameters.AddWithValue("@UrlImagen", modelo.UrlImagen);
                cmd.Parameters.AddWithValue("@NombreImagen", modelo.NombreImagen);
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("@Stock", modelo.Stock);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@IdProducto";
                outputParameter.SqlDbType = SqlDbType.Int;
                outputParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParameter);
                await cmd.ExecuteNonQueryAsync();

                int ProductoId = Convert.ToInt32(outputParameter.Value);
                modelo.IdProducto = ProductoId;

                return modelo;
                }            
            }
            catch (Exception)
            {

                throw;
            }
        }




        public async Task<bool> Editar(Productos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarProductos", conexion);
                cmd.Parameters.AddWithValue("@IdProducto", modelo.IdProducto);
                cmd.Parameters.AddWithValue("@Descripcion", modelo.Descripcion);
                cmd.Parameters.AddWithValue("@IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("@Precio", modelo.Precio);
                cmd.Parameters.AddWithValue("@UrlImagen", modelo.UrlImagen);
                cmd.Parameters.AddWithValue("@NombreImagen", modelo.NombreImagen);
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("@Stock", modelo.Stock);

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
                SqlCommand cmd = new SqlCommand("SPEliminarProductos", conexion);
                cmd.Parameters.AddWithValue("IdProducto", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }



        public Task<bool> Guardar(Productos modelo)
        {
            throw new NotImplementedException();
        }


        public async Task<IQueryable<Productos>> Consultar()
        {
            List<Productos> lista = new List<Productos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaProductos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Productos
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            UrlImagen = dr["UrlImagen"].ToString(),
                            NombreImagen = dr["NombreImagen"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                        });
                    }
                }
            }

            return lista.AsQueryable();
        }


        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();

                    // Obtener la fecha de inicio (una semana atrás)
                    DateTime fechaInicio = DateTime.Now.AddDays(-7);

                    // Comando SQL para llamar al procedimiento almacenado
                    using (var cmd = new SqlCommand("SPProductosTopUltimaSemana", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string producto = reader["Descripcion"].ToString();
                                int total = Convert.ToInt32(reader["Total"]);
                                resultado[producto] = total;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return resultado;
        }

        public async Task<Dictionary<string, int>> MisProductosTopUltimaSemana(DateTime fechaInicio, int idCliente)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    using (var cmd = new SqlCommand("SPMisProductosTopUltimaSemana", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string producto = reader["Descripcion"].ToString();
                                int total = Convert.ToInt32(reader["Total"]);
                                resultado[producto] = total;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return resultado;
        }


        public Task<IQueryable<Productos>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Productos> Buscar(string? Descripcion = null, string? NombreImagen = null, int? IdProducto = null)
        {
            Productos lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarProductos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", (object)Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProducto", (object)IdProducto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreImagen", (object)NombreImagen ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Productos
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            UrlImagen = dr["UrlImagen"].ToString(),
                            NombreImagen = dr["NombreImagen"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Productos> Verificar(string? Descripcion = null, string? NombreImagen = null, int? IdProducto = null)
        {
            Productos lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDescripcionProducto", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", (object)Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProducto", (object)IdProducto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreImagen", (object)NombreImagen ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Productos
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            UrlImagen = dr["UrlImagen"].ToString(),
                            NombreImagen = dr["NombreImagen"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                        };
                    }
                }
            }

            return lista;
        }

        public Task<List<Productos>> ConsultarLista()
        {
            throw new NotImplementedException();
        }

        public Task<List<Productos>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
