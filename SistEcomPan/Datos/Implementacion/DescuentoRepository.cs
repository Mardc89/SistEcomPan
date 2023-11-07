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
    public class DescuentoRepository:IGenericRepository<Descuentos>
    {
        private readonly string _cadenaSQL = "";

        public DescuentoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Descuentos>> Lista()
        {
            List<Descuentos> lista = new List<Descuentos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDescuentos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Descuentos
                        {
                            IdDescuento = Convert.ToInt32(dr["IdDescuento"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Descuento = Convert.ToDecimal(dr["Descuento"]),
                            Estado = Convert.ToBoolean(dr["Estado"])

                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Descuentos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarCategorias", conexion);
                cmd.Parameters.AddWithValue("IdProducto", modelo.IdProducto);
                cmd.Parameters.AddWithValue("Descuento", modelo.Descuento);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Descuentos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarDescuentos", conexion);
                cmd.Parameters.AddWithValue("IdDescuento", modelo.IdDescuento);
                cmd.Parameters.AddWithValue("IdProducto", modelo.IdProducto);
                cmd.Parameters.AddWithValue("Descuento", modelo.Descuento);
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
                SqlCommand cmd = new SqlCommand("SPEliminarDescuentos", conexion);
                cmd.Parameters.AddWithValue("IdDescuento", id);
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
