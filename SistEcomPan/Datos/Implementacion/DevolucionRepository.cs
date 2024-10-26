using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class DevolucionRepository : IDevolucionNew
    {
        private readonly string _cadenaSQL = "";

        public DevolucionRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }
        public Task<Devolucion> Buscar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Devolucion>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Devolucion>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<Devolucion> Crear(Devolucion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(Devolucion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(Devolucion modelo)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Devolucion>> Lista()
        {
            try
            {
                List<Devolucion> lista = new List<Devolucion>();
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPListaDevoluciones", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(new Devolucion
                            {
                                IdDevolucion = Convert.ToInt32(dr["IdDevolucion"]),
                                CodigoPedido = dr["CodigoPedido"].ToString(),
                                CodigoDevolucion = dr["CodigoDevolucion"].ToString(),
                                MontoPedido = Convert.ToDecimal(dr["MontoDePedido"]),
                                Descuento = Convert.ToDecimal(dr["Descuento"]),
                                MontoAPagar = Convert.ToDecimal(dr["MontoAPagar"]),
                                FechaDevolucion = Convert.ToDateTime(dr["FechaDevolucion"])
                            });
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Devolucion:{ ex.Message}");
                throw;
            }
        }

        public Task<IQueryable<Devolucion>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Devolucion> Registrar(Devolucion modelo, DataTable detalleDevolucion)
        {
            bool resultado = false;
            string Mensaje = "";
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarDevolucion", conexion);
                    cmd.Parameters.AddWithValue("@CodigoPedido", modelo.CodigoPedido);
                    cmd.Parameters.AddWithValue("@CodigoDevolucion", modelo.CodigoDevolucion);
                    cmd.Parameters.AddWithValue("@MontoDePedido", modelo.MontoPedido);
                    cmd.Parameters.AddWithValue("@Descuento", modelo.Descuento);
                    cmd.Parameters.AddWithValue("@MontoAPagar", modelo.MontoAPagar);
                    cmd.Parameters.AddWithValue("@DetalleDevolucion", detalleDevolucion);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdDevolucion";
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
                    int DevolucionId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdDevolucion = DevolucionId;

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return modelo;
        }

        public Task<Devolucion> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
