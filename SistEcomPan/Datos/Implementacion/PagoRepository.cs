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
    public class PagoRepository:IGenericRepository<Pagos>
    {
        private readonly string _cadenaSQL = "";

        public PagoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Pagos>> Lista()
        {
            List<Pagos> lista = new List<Pagos>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaPagos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Pagos
                        {
                            IdPago = Convert.ToInt32(dr["IdPago"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            MontoTotalPedido =Convert.ToDecimal(dr["MontoTotalPedido"]),
                            PagoDelCliente = Convert.ToDecimal(dr["PagoDelCliente"]),
                            VueltoDelCliente = Convert.ToDecimal(dr["VueltoDelCliente"]),
                            MontoDeuda = Convert.ToDecimal(dr["MontoDeuda"]),
                            FechaDeuda = Convert.ToDateTime(dr["FechaDeuda"])
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Pagos modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarPagos", conexion);
                cmd.Parameters.AddWithValue("IdPedido", modelo.IdPedido);
                cmd.Parameters.AddWithValue("MontoTotalPedido", modelo.MontoTotalPedido);
                cmd.Parameters.AddWithValue("PagoDelCliente", modelo.PagoDelCliente);
                cmd.Parameters.AddWithValue("VueltoDelCliente", modelo.VueltoDelCliente);
                cmd.Parameters.AddWithValue("MontoDeuda", modelo.MontoDeuda);
                cmd.Parameters.AddWithValue("FechaDeuda", modelo.FechaDeuda);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Pagos modelo)
        { 
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarPagos", conexion);
                cmd.Parameters.AddWithValue("IdPago", modelo.IdPago);
                cmd.Parameters.AddWithValue("IdPedido", modelo.IdPedido);
                cmd.Parameters.AddWithValue("MontoTotalPedido", modelo.MontoTotalPedido);
                cmd.Parameters.AddWithValue("PagoDelCliente", modelo.PagoDelCliente);
                cmd.Parameters.AddWithValue("VueltoDelCliente", modelo.VueltoDelCliente);
                cmd.Parameters.AddWithValue("MontoDeuda", modelo.MontoDeuda);
                cmd.Parameters.AddWithValue("FechaDeuda", modelo.FechaDeuda);

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
                SqlCommand cmd = new SqlCommand("SPEliminarPagos", conexion);
                cmd.Parameters.AddWithValue("IdPago", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<IQueryable<Pagos>> Consultar(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
