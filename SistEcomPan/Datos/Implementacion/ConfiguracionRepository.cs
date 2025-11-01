using Datos.Interfaces;
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
    public class ConfiguracionRepository : IConfiguracionRepository<Configuracion>
    {
        private readonly string _cadenaSQL = "";

        public ConfiguracionRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

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
