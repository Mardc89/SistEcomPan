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

namespace Datos.Implementacion
{
    public class ProductoRepository:IGenericRepository<Productos>
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
                            NombreImagen= dr["NombreFoto"].ToString(), 
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Productos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarProductos", conexion);
                cmd.Parameters.AddWithValue("Descripcion", modelo.Descripcion);
                cmd.Parameters.AddWithValue("IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("Precio", modelo.Precio);
                cmd.Parameters.AddWithValue("UrlImagen", modelo.UrlImagen);
                cmd.Parameters.AddWithValue("NombreImagen", modelo.NombreImagen);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("Stock", modelo.Stock);
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Productos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarProductos", conexion);
                cmd.Parameters.AddWithValue("IdProducto", modelo.IdProducto);
                cmd.Parameters.AddWithValue("Descripcion", modelo.Descripcion);
                cmd.Parameters.AddWithValue("IdCategoria", modelo.IdCategoria);
                cmd.Parameters.AddWithValue("Precio", modelo.Precio);
                cmd.Parameters.AddWithValue("UrlImagen", modelo.UrlImagen);
                cmd.Parameters.AddWithValue("NombreImagen", modelo.NombreImagen);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("Stock", modelo.Stock);
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);

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

        public Task<IQueryable<Productos>> Consultar(string consulta)
        {
            throw new NotImplementedException();
        }

        public Task<Productos> Crear(Productos modelo)
        {
            throw new NotImplementedException();
        }


        public Task<IQueryable<Productos>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Productos>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
