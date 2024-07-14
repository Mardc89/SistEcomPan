using Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq.Expressions;

namespace Datos.Implementacion
{
    public class UsuarioRepository:IGenericRepository<Usuarios>
    {
        private readonly string _cadenaSQL = "";
        private readonly IHostEnvironment _environment;
        

        public UsuarioRepository(IConfiguration configuration, IHostEnvironment environment)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
            _environment = environment;
        }

        public async Task<List<Usuarios>> Lista()
        {
            List<Usuarios> lista = new List<Usuarios>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Usuarios
                        {
                            IdUsuario = Convert.ToInt32(dr["Idusuario"]),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),                        
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(), 
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()                         
                        });
                    }
                }
            }

            return lista;
        }






        public async Task<bool> Editar(Usuarios modelo)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPEditarUsuario", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", modelo.IdUsuario);
                    cmd.Parameters.AddWithValue("@Dni", modelo.Dni);
                    cmd.Parameters.AddWithValue("@Nombres", modelo.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", modelo.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", modelo.Correo);
                    cmd.Parameters.AddWithValue("@NombreUsuario", modelo.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Clave", modelo.Clave);
                    cmd.Parameters.AddWithValue("@IdRol", modelo.IdRol);
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
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPEliminarUsuario", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", id);
                    cmd.CommandType = CommandType.StoredProcedure;

                    int filaAfectada = await cmd.ExecuteNonQueryAsync();

                    if (filaAfectada > 0)
                        return true;
                    else
                        return false;


                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Usuarios> Crear(Usuarios modelo)
        {
            try
            {

                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarUsuario", conexion);
                    cmd.Parameters.AddWithValue("@Dni", modelo.Dni);
                    cmd.Parameters.AddWithValue("@Nombres", modelo.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", modelo.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", modelo.Correo);
                    cmd.Parameters.AddWithValue("@NombreUsuario", modelo.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Clave", modelo.Clave);
                    cmd.Parameters.AddWithValue("@IdRol", modelo.IdRol);
                    cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                    cmd.Parameters.AddWithValue("@UrlFoto", modelo.UrlFoto);
                    cmd.Parameters.AddWithValue("@NombreFoto", modelo.NombreFoto);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@IdUsuario";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);
                    await cmd.ExecuteNonQueryAsync();

                    int UsuarioId = Convert.ToInt32(outputParameter.Value);
                    modelo.IdUsuario = UsuarioId;

                    return modelo;

                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<Usuarios> Editar(Usuarios entidad, Stream foto)
        {
            throw new NotImplementedException();
        }





        public async Task<IQueryable<Usuarios>> Consultar()
        {
            List<Usuarios> lista = new List<Usuarios>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Usuarios
                        {
                            IdUsuario = Convert.ToInt32(dr["Idusuario"]),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        });
                    }
                }
            }

            return lista.AsQueryable();
        }

        public Task<bool> Guardar(Usuarios modelo)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Usuarios>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuarios> Buscar(string? Correo = null, string? Clave = null, int? IdUsuario = null)
        {
            Usuarios lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdUsuario", (object)IdUsuario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Usuarios
                        {
                            IdUsuario = Convert.ToInt32(dr["Idusuario"]),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }

        public async Task<Usuarios> Verificar(string? Correo = null, string? Clave = null, int? IdUsuario = null)
        {
            Usuarios lista = null;
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarUsuarioCorreo", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", (object)Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdUsuario", (object)IdUsuario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Clave", (object)Clave ?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista = new Usuarios
                        {
                            IdUsuario = Convert.ToInt32(dr["Idusuario"]),
                            Dni = dr["Dni"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto = dr["UrlFoto"].ToString(),
                            NombreFoto = dr["NombreFoto"].ToString()
                        };
                    }
                }
            }

            return lista;
        }


        public Task<List<Usuarios>> Consultar(string? c = null, string? p = null, string? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
