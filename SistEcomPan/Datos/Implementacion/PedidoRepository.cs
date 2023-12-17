using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class PedidoRepository : IPedidoNew
    {
        private readonly string _cadenaSQL="";

        public PedidoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Pedidos>> Lista()
        {
            List<Pedidos> lista = new List<Pedidos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaPedidos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Pedidos
                        {
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Codigo = dr["Codigo"].ToString(),
                            MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                            Estado = dr["Estado"].ToString(),                          
                            FechaPedido = Convert.ToDateTime(dr["FechaPedido"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Editar(Pedidos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarPedidos", conexion);
                cmd.Parameters.AddWithValue("IdPedido", modelo.IdPedido);
                cmd.Parameters.AddWithValue("IdCliente", modelo.IdCliente);
                cmd.Parameters.AddWithValue("Codigo", modelo.Codigo);
                cmd.Parameters.AddWithValue("MontoTotal", modelo.MontoTotal);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("FechaPedido", modelo.FechaPedido);

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
                SqlCommand cmd = new SqlCommand("SPEliminarPedidos", conexion);
                cmd.Parameters.AddWithValue("IdPedido", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public async Task<Pedidos> Registrar(Pedidos modelo, DataTable DetallePedido)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarPedidos", conexion);
                    cmd.Parameters.AddWithValue("@IdCliente", modelo.IdCliente);
                    cmd.Parameters.AddWithValue("@codigo", modelo.Codigo);
                    cmd.Parameters.AddWithValue("@MontoTotal", modelo.MontoTotal);
                    cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                    cmd.Parameters.AddWithValue("@FechaPedido", modelo.FechaPedido);
                    cmd.Parameters.AddWithValue("@DetallePedido", DetallePedido);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdPedido";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int PedidoId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdPedido = PedidoId;

                    return modelo;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<Pedidos>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(Pedidos modelo)
        {
            throw new NotImplementedException();
        }


        public Task<Pedidos> Crear(Pedidos modelo)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Pedidos>> Consultar()
        {
            List<Pedidos> lista = new List<Pedidos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaPedidos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Pedidos
                        {
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Codigo = dr["Codigo"].ToString(),
                            MontoTotal =Convert.ToDecimal(dr["MontoTotal"]),                  
                            Estado = dr["Estado"].ToString(),
                            FechaPedido = Convert.ToDateTime(dr["FechaRegistro"])
                        });
                    }
                }
            }

            return lista.AsQueryable();
        }

        public Task<IQueryable<Pedidos>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }


    }
}
