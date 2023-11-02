using Datos.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class DataBaseService : IDataBaseService
    {
        private readonly string _connectionString;
        public DataBaseService(IConfiguration configuration )
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
                
        }
        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
