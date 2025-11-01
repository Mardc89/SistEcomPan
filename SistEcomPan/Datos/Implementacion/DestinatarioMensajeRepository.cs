using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Datos.Interfaces;

namespace Datos.Implementacion
{
    public class DestinatarioMensajeRepository:IDestinatarioMensajeRepository
    {
        private readonly string _cadenaSQL = "";

        public DestinatarioMensajeRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<DestinatarioMensaje>> Lista()
        {
            List<DestinatarioMensaje> lista = new List<DestinatarioMensaje>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDestinatarioMensaje", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DestinatarioMensaje
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            IdDestinatario = Convert.ToInt32(dr["IdDestinatario"]),
                            Destinatario = dr["Destinatario"].ToString()

                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(DestinatarioMensaje modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarDestinatarioMensaje", conexion);
                cmd.Parameters.AddWithValue("IdDestinatario", modelo.IdDestinatario);
                cmd.Parameters.AddWithValue("Destinatario", modelo.Destinatario);

                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(DestinatarioMensaje modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarDestinatarioMensaje", conexion);
                cmd.Parameters.AddWithValue("IdMensaje", modelo.IdMensaje);
                cmd.Parameters.AddWithValue("IdDestinatario", modelo.IdDestinatario);
                cmd.Parameters.AddWithValue("Destinatario", modelo.Destinatario);
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
                SqlCommand cmd = new SqlCommand("SPEliminarDestinatarioMensaje", conexion);
                cmd.Parameters.AddWithValue("IdMensaje", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }



        public async Task<DestinatarioMensaje> Crear(DestinatarioMensaje modelo)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarMensajes", conexion);
                    cmd.Parameters.AddWithValue("@IdDestinatario", modelo.IdDestinatario);
                    cmd.Parameters.AddWithValue("@Destinatario", modelo.Destinatario);
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




        public async Task<DestinatarioMensaje> Buscar(string? Destinatario = null, string? Asunto = null, int? IdMensaje = null)
        {
            DestinatarioMensaje lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarDestinatarioMensaje", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensaje", (object)IdMensaje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Destinatario", (object)Destinatario ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new DestinatarioMensaje
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            IdDestinatario = Convert.ToInt32(dr["IdDestinatario"]),
                            Destinatario = dr["Destinatario"].ToString(),
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<DestinatarioMensaje> Verificar(string? Destinatario = null, string? Asunto = null, int? IdDestinatario = null)
        {
            DestinatarioMensaje lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                SqlCommand cmd = new SqlCommand("SPVerificarDestinatarioMensaje", conexion);
                conexion.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDestinatario", (object)IdDestinatario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Destinatario", (object)Destinatario ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new DestinatarioMensaje
                        {
                            IdMensaje = Convert.ToInt32(dr["IdMensaje"]),
                            IdDestinatario = Convert.ToInt32(dr["IdDestinatario"]),
                            Destinatario = dr["Destinatario"].ToString(),
                        };
                    }
                }
            }

            return lista;
        }


    }
}
