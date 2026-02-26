using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Datos.Implementacion

{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _cadenaSQL = "";

        public ClienteRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Clientes>> Lista()
        {
            List<Clientes> lista = new List<Clientes>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaClientes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Clientes
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            TipoCliente = dr["TipoCliente"].ToString(),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<Clientes> Crear(Clientes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPRegistrarCliente", conexion);
                cmd.Parameters.AddWithValue("TipoCliente", modelo.TipoCliente);
                cmd.Parameters.AddWithValue("Dni", modelo.Dni);
                cmd.Parameters.AddWithValue("Nombres", modelo.Nombres);
                cmd.Parameters.AddWithValue("Apellidos", modelo.Apellidos);
                cmd.Parameters.AddWithValue("Correo", modelo.Correo);
                cmd.Parameters.AddWithValue("Direccion", modelo.Direccion);
                cmd.Parameters.AddWithValue("Telefono", modelo.Telefono);
                cmd.Parameters.AddWithValue("IdDistrito", modelo.IdDistrito);
                cmd.Parameters.AddWithValue("NombreUsuario", modelo.NombreUsuario);
                cmd.Parameters.AddWithValue("Clave", modelo.Clave);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("UrlFoto", modelo.UrlFoto);
                cmd.Parameters.AddWithValue("NombreFoto", modelo.NombreFoto);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@IdCliente";
                outputParameter.SqlDbType = SqlDbType.Int;
                outputParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParameter);
                await cmd.ExecuteNonQueryAsync();

                int ClienteId = Convert.ToInt32(outputParameter.Value);
                modelo.IdCliente = ClienteId;

                return modelo;


            }
        }




        public async Task<bool> Editar(Clientes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarClientes", conexion);
                cmd.Parameters.AddWithValue("@IdCliente", modelo.IdCliente);
                cmd.Parameters.AddWithValue("@TipoCliente", modelo.TipoCliente);
                cmd.Parameters.AddWithValue("@Dni", modelo.Dni);
                cmd.Parameters.AddWithValue("@Nombres", modelo.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", modelo.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", modelo.Correo);
                cmd.Parameters.AddWithValue("@Direccion", modelo.Direccion);
                cmd.Parameters.AddWithValue("@Telefono", modelo.Telefono);
                cmd.Parameters.AddWithValue("@IdDistrito", modelo.IdDistrito);
                cmd.Parameters.AddWithValue("@NombreUsuario", modelo.NombreUsuario);
                cmd.Parameters.AddWithValue("@Clave", modelo.Clave);
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("@UrlFoto", modelo.UrlFoto);
                cmd.Parameters.AddWithValue("@NombreFoto", modelo.NombreFoto);

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
                SqlCommand cmd = new SqlCommand("SPEliminarCliente", conexion);
                cmd.Parameters.AddWithValue("@IdCliente", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public async Task<Clientes> ConsultarCliente(string? Correo = null, string? Clave = null, int? IdCliente = null)
        {
            Clientes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarClientes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdCliente", (object)IdCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Clientes
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            TipoCliente = dr["TipoCliente"].ToString(),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Clientes> ConsultarCorreo(string? Correo = null, string? Clave = null, int? IdCliente = null)
        {
            Clientes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarCorreo", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdCliente", (object)IdCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Clientes
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            TipoCliente = dr["TipoCliente"].ToString(),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }




        public async Task<Clientes> Buscar(string? Correo = null, string? Clave = null, int? IdCliente = null, string? Apellidos = null, string? Nombres = null, string? Dni = null)
        {
            Clientes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarClientes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdCliente", (object)IdCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)Apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombres", (object)Nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Dni", (object)Dni ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Clientes
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            TipoCliente = dr["TipoCliente"].ToString(),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Clientes> Verificar(string? Correo = null, string? Clave = null, int? IdCliente = null)
        {
            Clientes lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarCorreo", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdCliente", (object)IdCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Clientes
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            TipoCliente = dr["TipoCliente"].ToString(),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                            dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Clientes> BuscarCliente(string? nombreCompleto = null)
        {
            Clientes? lista = null;
            await using var conexion = new SqlConnection(_cadenaSQL);
            await conexion.OpenAsync();

            await using var cmd = new SqlCommand("SPBuscarCliente", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200)
                          .Value = (object?)nombreCompleto ?? DBNull.Value;

            await using var dr = await cmd.ExecuteReaderAsync();


            if (await dr.ReadAsync())
            {
                lista = new Clientes
                {
                    IdCliente = Convert.ToInt32(dr["IdCliente"]),
                    TipoCliente = dr["TipoCliente"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Correo = dr["Correo"].ToString(),
                    Direccion = dr["Direccion"].ToString(),
                    Telefono = dr["Telefono"].ToString(),
                    IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                    NombreUsuario = dr["NombreUsuario"].ToString(),
                    Clave = dr["Clave"].ToString(),
                    FechaRegistro = dr.IsDBNull(dr.GetOrdinal("FechaRegistro")) ? null : DateTime.SpecifyKind(
                    dr.GetDateTime(dr.GetOrdinal("FechaRegistro")), DateTimeKind.Utc),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    UrlFoto = dr["UrlFoto"].ToString(),
                    NombreFoto = dr["NombreFoto"].ToString()
                };
            }

            return lista;
        }
    }
}
        
    


