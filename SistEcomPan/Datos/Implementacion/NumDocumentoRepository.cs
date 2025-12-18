using Datos.Interfaces;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Datos.Implementacion
{
    public class NumDocumentoRepository : INumeroDocumento
    {
        private readonly string _cadenaSQL = "";

        public NumDocumentoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<NumeroDocumento> Buscar(string? Gestion = null, string? Fecha = null, int? IdNumeroDocumento = null)
        {
            NumeroDocumento lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarNumeroDocumento", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                DateTime parseDate;
                cmd.Parameters.AddWithValue("@Gestion", (object)Gestion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdNumeroDocumento", (object)IdNumeroDocumento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha",!DateTime.TryParse(Fecha ,out parseDate)?DBNull.Value:parseDate);

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista=new NumeroDocumento
                        {
                            IdNumeroDocumento = Convert.ToInt32(dr["IdNumeroDocumento"]),
                            UltimoNumero = Convert.ToInt32(dr["UltimoNumero"]),
                            CantidadDeDigitos = Convert.ToInt32(dr["CantidadDeDigitos"]),
                            Gestion = dr["Gestion"].ToString(),
                            FechaActualizacion = dr.IsDBNull(dr.GetOrdinal("FechaActualizacion")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaActualizacion")), DateTimeKind.Utc),
                        };
                    }
                }
            }

            return lista;
        }



        public async Task<bool> Editar(NumeroDocumento modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarNumeroDocumento", conexion);
                cmd.Parameters.AddWithValue("IdNumeroDocumento", modelo.IdNumeroDocumento);
                cmd.Parameters.AddWithValue("UltimoNumero", modelo.UltimoNumero);
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
