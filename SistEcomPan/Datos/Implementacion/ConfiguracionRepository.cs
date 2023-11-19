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

namespace Datos.Implementacion
{
    public class ConfiguracionRepository : IGenericRepository<Configuracion>
    {
        private readonly string _cadenaSQL = "";

        public ConfiguracionRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<IQueryable<Configuracion>> Consultar(string consulta)
        {
            List<Configuracion> lista = new List<Configuracion>();

            using (var connection = new SqlConnection(_cadenaSQL))
            {
                using (var command = new SqlCommand("BuscarCategoria", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Texto", SqlDbType.NVarChar, 100).Value = consulta;

                    try
                    {
                        connection.Open();
                        SqlDataReader dr = command.ExecuteReader();

                        while (dr.Read())
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



        public Task<bool> Delete(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(Configuracion modelo)
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

        public Task<Configuracion> Obtener(Configuracion modelo)
        {
            throw new NotImplementedException();
        }
    }
}
