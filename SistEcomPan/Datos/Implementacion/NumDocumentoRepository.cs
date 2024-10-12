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
    public class NumDocumentoRepository : IGenericRepository<NumeroDocumento>
    {
        private readonly string _cadenaSQL = "";

        public NumDocumentoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public Task<NumeroDocumento> Buscar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<NumeroDocumento>> Consultar()
        {
            List<NumeroDocumento> lista = new List<NumeroDocumento>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaNumeroDocumento", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new NumeroDocumento
                        {
                            IdNumeroDocumento = Convert.ToInt32(dr["IdNumeroDocumento"]),
                            UltimoNumero = Convert.ToInt32(dr["UltimoNumero"]),
                            CantidadDeDigitos = Convert.ToInt32(dr["CantidadDeDigitos"]),
                            Gestion = dr["Gestion"].ToString(),
                            FechaActualizacion = Convert.ToDateTime(dr["FechaActualizacion"])
                        });
                    }
                }
            }

            return lista.AsQueryable();

        }

        public Task<List<NumeroDocumento>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<NumeroDocumento>> ConsultarLista()
        {
            throw new NotImplementedException();
        }

        public Task<NumeroDocumento> Crear(NumeroDocumento modelo)
        {
            throw new NotImplementedException();
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

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(NumeroDocumento modelo)
        {
            throw new NotImplementedException();
        }

        public Task<List<NumeroDocumento>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<NumeroDocumento>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public Task<NumeroDocumento> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
