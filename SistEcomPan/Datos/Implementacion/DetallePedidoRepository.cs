using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Datos.Implementacion
{
    public class DetallePedidoRepository : IDetallePedidoRepository
    {
        private readonly string _cadenaSQL = "";

        public DetallePedidoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public async Task<List<DetallePedido>> Lista()
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDetallePedidos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DetallePedido
                        {
                            IdDetallePedido = Convert.ToInt32(dr["IdDetallePedido"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<DetallePedido> Buscar(string? codigo = null, string? estado = null, int? idPedido = null)
        {
            DetallePedido lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDetallePedido", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPedido", (object)idPedido ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new DetallePedido
                        {
                            IdDetallePedido = Convert.ToInt32(dr["IdDetallePedido"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total= Convert.ToDecimal(dr["Total"])
                        };
                    }
                }
            }

            return lista;
        }



        public async Task<List<DetallePedido>> ConsultarDetallePedido(int IdPedido)
        {

            List<DetallePedido> lista = new List<DetallePedido>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDetallePedido", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPedido", IdPedido);


                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DetallePedido
                        {
                            IdDetallePedido = Convert.ToInt32(dr["IdDetallePedido"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<List<DetallePedido>> Consultar(string? c = null, string? m = null, string? p = null, int? IdPedido= null)
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDetallePedido", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPedido", Convert.ToInt32(IdPedido));


                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DetallePedido
                        {
                            IdDetallePedido = Convert.ToInt32(dr["IdDetallePedido"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
