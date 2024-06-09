using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
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
            bool resultado = false;
            string Mensaje = "";
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarPedidos", conexion);
                    cmd.Parameters.AddWithValue("@IdCliente", modelo.IdCliente);
                    cmd.Parameters.AddWithValue("@Codigo", modelo.Codigo);
                    cmd.Parameters.AddWithValue("@MontoTotal", modelo.MontoTotal);
                    cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                    cmd.Parameters.AddWithValue("@DetallePedido", DetallePedido);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdPedido";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    SqlParameter outputParameter2 = new SqlParameter();
                    outputParameter2.ParameterName = "@Resultado";
                    outputParameter2.SqlDbType = SqlDbType.Bit;
                    outputParameter2.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter2);

                    await cmd.ExecuteNonQueryAsync();

                    resultado = Convert.ToBoolean(cmd.Parameters["@Resultado"].Value);
                    int PedidoId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdPedido = PedidoId;

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            } 
            return modelo;
        }



        public Task<bool> Guardar(Pedidos modelo)
        {
            throw new NotImplementedException();
        }


        public Task<Pedidos> Crear(Pedidos modelo)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Pedidos>> ConsultarPedido(DateTime?fechaPedido=null)
        {


            List<Pedidos> lista = new List<Pedidos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarPedidos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaPedido",(object)fechaPedido??DBNull.Value);

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
                            FechaPedido = Convert.ToDateTime(dr["FechaPedido"])
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

        public Task<IQueryable<Pedidos>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<List<Pedidos>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            throw new NotImplementedException();
        }

        public Task<Pedidos> Buscar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<Pedidos> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
