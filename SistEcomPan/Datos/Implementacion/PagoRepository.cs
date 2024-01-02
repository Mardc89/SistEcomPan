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
                            MontoDePedido = Convert.ToDecimal(dr["MontoDePedido"]),
                            Descuento = Convert.ToDecimal(dr["Descuento"]),
                            MontoTotalDePago = Convert.ToDecimal(dr["MontoTotalDePago"]),
                            MontoDeuda = Convert.ToDecimal(dr["MontoDeuda"]),
                            FechaDePago= Convert.ToDateTime(dr["FechaDePago"]),
                            Estado = dr["Estado"].ToString()
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
                cmd.Parameters.AddWithValue("MontoDePedido", modelo.MontoDePedido);
                cmd.Parameters.AddWithValue("Descuento", modelo.Descuento);
                cmd.Parameters.AddWithValue("MontoTotalDePago", modelo.MontoTotalDePago);
                cmd.Parameters.AddWithValue("MontoDeuda", modelo.MontoDeuda);
                cmd.Parameters.AddWithValue("FechaPago", modelo.FechaDePago);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
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
                cmd.Parameters.AddWithValue("MontoDePedido", modelo.MontoDePedido);
                cmd.Parameters.AddWithValue("Descuento", modelo.Descuento);
                cmd.Parameters.AddWithValue("MontoTotalDePago", modelo.MontoTotalDePago);
                cmd.Parameters.AddWithValue("MontoDeuda", modelo.MontoDeuda);
                cmd.Parameters.AddWithValue("FechaPago", modelo.FechaDePago);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);

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

        public Task<Pagos> Crear(Pagos modelo)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Pagos>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Pagos>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
