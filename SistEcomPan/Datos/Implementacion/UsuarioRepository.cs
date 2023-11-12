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

namespace Datos.Implementacion
{
    public class UsuarioRepository:IGenericRepository<Usuarios>,IUsuarioRepository
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

        public async Task<bool> Guardar(Usuarios modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarUsuarios", conexion);
                cmd.Parameters.AddWithValue("Dni", modelo.Dni);
                cmd.Parameters.AddWithValue("Nombres", modelo.Nombres);
                cmd.Parameters.AddWithValue("Apellidos", modelo.Apellidos);
                cmd.Parameters.AddWithValue("Correo", modelo.Correo);   
                cmd.Parameters.AddWithValue("NombreUsuario", modelo.NombreUsuario);    
                cmd.Parameters.AddWithValue("Clave", modelo.Clave);
                cmd.Parameters.AddWithValue("IdRol", modelo.IdRol);                       
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("UrlFoto", modelo.UrlFoto);
                cmd.Parameters.AddWithValue("NombreFoto", modelo.NombreFoto);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }




        public async Task<bool> Editar(Usuarios modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarUsuarios", conexion);
                cmd.Parameters.AddWithValue("IdUsuario", modelo.IdUsuario);
                cmd.Parameters.AddWithValue("Dni", modelo.Dni);
                cmd.Parameters.AddWithValue("Nombres", modelo.Nombres);
                cmd.Parameters.AddWithValue("Apellidos", modelo.Apellidos);
                cmd.Parameters.AddWithValue("Correo", modelo.Correo);
                cmd.Parameters.AddWithValue("NombreUsuario", modelo.NombreUsuario);
                cmd.Parameters.AddWithValue("Clave", modelo.Clave);
                cmd.Parameters.AddWithValue("IdRol", modelo.IdRol);
                cmd.Parameters.AddWithValue("FechaRegistro", modelo.FechaRegistro);
                cmd.Parameters.AddWithValue("Estado", modelo.Estado);
                cmd.Parameters.AddWithValue("UrlFoto", modelo.UrlFoto);
                cmd.Parameters.AddWithValue("NombreFoto", modelo.NombreFoto);

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
                SqlCommand cmd = new SqlCommand("SPEliminarUsuarios", conexion);
                cmd.Parameters.AddWithValue("IdUsuario", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<Usuarios> Crear(Usuarios entidad,IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<Usuarios> Editar(Usuarios entidad, Stream foto)
        {
            throw new NotImplementedException();
        }
    }
}
