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
using System.Linq.Expressions;

namespace Datos.Implementacion
{
     public class MensajeRepository:IDestinatarioNew
    {
        private readonly string _cadenaSQL = "";

        public MensajeRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Mensajes>> Lista()
        {
            List<Mensajes> lista = new List<Mensajes>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaMensajes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            IdRemitente = Convert.ToInt32(dr["IdRemitente"]),
                            Asunto = dr["Asunto"].ToString(),
                            Cuerpo = dr["Cuerpo"].ToString(),
                            Remitente = dr["Remitente"].ToString(),
                            FechaDeMensaje = Convert.ToDateTime(dr["FechaDeMensaje"])

                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar (Mensajes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarCategorias", conexion);
                cmd.Parameters.AddWithValue("IdCliente", modelo.IdRemitente);
                cmd.Parameters.AddWithValue("Asunto", modelo.Asunto);
                cmd.Parameters.AddWithValue("Cuerpo", modelo.Cuerpo);
           

                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Mensajes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarMensajes", conexion);
                cmd.Parameters.AddWithValue("IdMensaje", modelo.IdMensaje);
                cmd.Parameters.AddWithValue("Cuerpo", modelo.Cuerpo);
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
                SqlCommand cmd = new SqlCommand("SPEliminarMensajes", conexion);
                cmd.Parameters.AddWithValue("@IdMensaje", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<IQueryable<Mensajes>> Consultar(Expression<Func<Mensajes, bool>> filtro = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Mensajes> Crear(Mensajes modelo,DestinatarioMensaje destino)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarMensajes", conexion);   
                    cmd.Parameters.AddWithValue("@IdRemitente", modelo.IdRemitente);             
                    cmd.Parameters.AddWithValue("@Asunto", modelo.Asunto);
                    cmd.Parameters.AddWithValue("@Cuerpo", modelo.Cuerpo);                 
                    cmd.Parameters.AddWithValue("@Remitente", modelo.Remitente);
                    cmd.Parameters.AddWithValue("@IdRespuestaMensaje", modelo.IdRespuestaMensaje);
                    cmd.Parameters.AddWithValue("@IdDestinatario", destino.IdDestinatario);
                    cmd.Parameters.AddWithValue("@Destinatario", destino.Destinatario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdMensaje";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int MensajeId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdMensaje = MensajeId;

                    return modelo;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Mensajes> CrearRespuestaMensaje(Mensajes modelo, DestinatarioMensaje destino)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarMensajes", conexion);
                    cmd.Parameters.AddWithValue("@IdRemitente", modelo.IdRemitente);
                    cmd.Parameters.AddWithValue("@Asunto", modelo.Asunto);
                    cmd.Parameters.AddWithValue("@Cuerpo", modelo.Cuerpo);
                    cmd.Parameters.AddWithValue("@Remitente", modelo.Remitente);
                    cmd.Parameters.AddWithValue("@IdRespuestaMensaje", modelo.IdRespuestaMensaje);
                    cmd.Parameters.AddWithValue("@IdDestinatario", destino.IdDestinatario);
                    cmd.Parameters.AddWithValue("@Destinatario", destino.Destinatario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdMensaje";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int MensajeId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdMensaje = MensajeId;

                    return modelo;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Task<IQueryable<Mensajes>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Mensajes>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Mensajes> Buscar(string? Cuerpo = null, string? Asunto = null, int? IdMensaje = null)
        {
            Mensajes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarMensajes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensaje", (object)IdMensaje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Asunto", (object)Asunto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", (object)Cuerpo ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            Remitente = dr["Remitente"].ToString(),
                            IdRemitente = Convert.ToInt32(dr["IdRemitente"]),
                            Asunto = dr["Asunto"].ToString(),
                            Cuerpo = dr["Cuerpo"].ToString(),
                            IdRespuestaMensaje = dr["IdRespuestaMensaje"] as int?,
                            FechaDeMensaje = Convert.ToDateTime(dr["FechaDeMensaje"])
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Mensajes> Verificar(string? Cuerpo = null, string? Asunto = null, int? IdMensaje = null)
        {
            Mensajes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                SqlCommand cmd = new SqlCommand("SPVerificarMensajes", conexion);
                conexion.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensaje", (object)IdMensaje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Asunto", (object)Asunto ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Mensajes
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            IdRemitente = Convert.ToInt32(dr["IdCliente"]),
                            Asunto = dr["Asunto"].ToString(),
                            Cuerpo = dr["Cuerpo"].ToString(),
                            FechaDeMensaje = Convert.ToDateTime(dr["Fecha"])
                        };
                    }
                }
            }

            return lista;
        }

        public Task<List<Mensajes>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<Mensajes> Crear(Mensajes modelo)
        {
            throw new NotImplementedException();
        }
    }
}
