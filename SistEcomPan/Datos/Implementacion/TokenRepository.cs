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

namespace Datos.Implementacion
{
    public class TokenRepository:IGenericRepository<Tokens>
    {
        private readonly string _cadenaSQL = "";
        public TokenRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public async Task<Tokens> Buscar(string? c = null, string? Token = null, int? IdToken = null)
        {
            Tokens lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarToken", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdToken", (object)IdToken ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Token", (object)Token ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Tokens
                        {
                            IdToken = Convert.ToInt32(dr["IdToken"]),
                            Perfil = dr["Perfil"].ToString(),
                            IdPerfil = Convert.ToInt32(dr["IdPerfil"]),
                            Token = dr["Token"].ToString(),
                            Expiracion = Convert.ToDateTime(dr["Expiracion"]),
                            Creacion = Convert.ToDateTime(dr["Creacion"])
                        };
                    }
                }
            }

            return lista;
        }

        public Task<List<Tokens>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Tokens>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<Tokens> Crear(Tokens modelo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Editar(Tokens modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarTokens", conexion);
                cmd.Parameters.AddWithValue("Idtoken", modelo.IdToken);
                cmd.Parameters.AddWithValue("Perfil", modelo.Perfil);
                cmd.Parameters.AddWithValue("IdPerfil", modelo.IdPerfil);
                cmd.Parameters.AddWithValue("Token", modelo.Token);
                cmd.Parameters.AddWithValue("Expiracion", modelo.Expiracion);
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

        public async Task<bool> Guardar(Tokens modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarToken", conexion);
                cmd.Parameters.AddWithValue("@Perfil", modelo.Perfil);
                cmd.Parameters.AddWithValue("@IdPerfil", modelo.IdPerfil);
                cmd.Parameters.AddWithValue("@Token", modelo.Token);
                cmd.Parameters.AddWithValue("@Expiracion", modelo.Expiracion);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<List<Tokens>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Tokens>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public Task<Tokens> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
