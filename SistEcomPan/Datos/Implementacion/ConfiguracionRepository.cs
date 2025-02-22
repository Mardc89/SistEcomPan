﻿using Datos.Interfaces;
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
    public class ConfiguracionRepository : IGenericRepository<Configuracion>
    {
        private readonly string _cadenaSQL = "";

        public ConfiguracionRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<IQueryable<Configuracion>> Obtener(string consulta)
        {
            List<Configuracion> lista = new List<Configuracion>();

            using (var connection = new SqlConnection(_cadenaSQL))
            {
                using (var command = new SqlCommand("BuscarServicio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Texto", SqlDbType.NVarChar, 100).Value = consulta;

                    try
                    {
                        connection.Open();
                        SqlDataReader dr = command.ExecuteReader();

                        while (await dr.ReadAsync())
                        {
                            lista.Add(new Configuracion
                            {
                                Recurso = dr["Recurso"].ToString(),
                                Propiedad = dr["Propiedad"].ToString(),
                                Valor = dr["Valor"].ToString()
                            });

                        }

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        // Manejo de excepciones
                        Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                    }
                }
            }

            return lista.AsQueryable();

        }

        public Task<IQueryable<Configuracion>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<Configuracion> Crear(Configuracion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(Configuracion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(Configuracion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<List<Configuracion>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<Configuracion> Buscar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<Configuracion> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Configuracion>> ConsultarLista()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Configuracion>> Consultar(string? Recurso = null, string? p = null, string? m = null, int? d = null)
        {
            List<Configuracion> lista = new List<Configuracion>();

            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPConsultarConfiguracion", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Recurso", (object)Recurso?? DBNull.Value);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(new Configuracion
                            {
                                Recurso = dr["Recurso"].ToString(),
                                Propiedad = dr["Propiedad"].ToString(),
                                Valor = dr["Valor"].ToString()
                            });

                        }

                }
            }

            return lista;
        }
    }
}
