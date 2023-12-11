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

namespace Datos.Implementacion
{
    public class ClienteRepository:IGenericRepository<Clientes>
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
                            IdDistrito =Convert.ToInt32(dr["IdDistrito"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            Clave = dr["Clave"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            UrlFoto= dr["UrlFoto"].ToString(),
                            NombreFoto= dr["NombreFoto"].ToString()
                        }); 
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Guardar(Clientes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPGuardarClientes", conexion);
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




        public async Task<bool> Editar(Clientes modelo)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEditarClientes", conexion);
                cmd.Parameters.AddWithValue("IdCliente", modelo.IdCliente);
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

        public async Task<bool> Eliminar(int id)
        {
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPEliminarClientes", conexion);
                cmd.Parameters.AddWithValue("IdCliente", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int filaAfectada = await cmd.ExecuteNonQueryAsync();

                if (filaAfectada > 0)
                    return true;
                else
                    return false;


            }
        }

        public Task<Clientes> Crear(Clientes modelo)
        {
            throw new NotImplementedException();
        }


        public Task<IQueryable<Clientes>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Clientes>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }

}
