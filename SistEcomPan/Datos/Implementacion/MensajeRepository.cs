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
     public class MensajeRepository
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
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Asunto = dr["Asunto"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha"))

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
                cmd.Parameters.AddWithValue("IdCliente", modelo.IdCliente);
                cmd.Parameters.AddWithValue("Asunto", modelo.Asunto);
                cmd.Parameters.AddWithValue("Descripcion", modelo.Descripcion);
                cmd.Parameters.AddWithValue("Fecha", modelo.Fecha);

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
                cmd.Parameters.AddWithValue("IdCliente", modelo.IdCliente);
                cmd.Parameters.AddWithValue("Descuento", modelo.Asunto);
                cmd.Parameters.AddWithValue("Asunto", modelo.Descripcion);
                cmd.Parameters.AddWithValue("Fecha", modelo.Fecha);
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
                SqlCommand cmd = new SqlCommand("SPEliminarMensajes", conexion);
                cmd.Parameters.AddWithValue("IdMensaje", id);
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
