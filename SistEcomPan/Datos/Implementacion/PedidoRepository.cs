using Datos.Interfaces;
using Entidades;
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
    public class PedidoRepository : IPedidoRepository
    {
        private readonly string _cadenaSQL="";

        public PedidoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
                
        }

        public Task<bool> Delete(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(Pedidos modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(Pedidos modelo)
        {
            throw new NotImplementedException();
        }

        public Task<List<Pedidos>> Lista()
        {
            throw new NotImplementedException();
        }

        public async Task<Pedidos> Registrar(Pedidos entidad)
        {
            using (var conexion = new SqlConnection(_cadenaSQL)){
                        conexion.Open();
                using (var transaction =conexion.BeginTransaction()) {
                    try
                    {
                        foreach(DetallePedido dp in entidad.DetallePedido)
                        {
                            string stockQuery = "SELECT FROM Producto WHERE ProductoID = @productoId";
                            using (var stockCommand = new SqlCommand(stockQuery,conexion, transaction))
                            {
                                stockCommand.Parameters.AddWithValue("@productoId",dp.IdProducto);
                                int stockDisponible = (int)stockCommand.ExecuteScalar();

                            }
                            

                        }

                    }
                    catch
                    {

                        throw;
                    }

                }


            }
        }

    public Task<List<Pedidos>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            throw new NotImplementedException();
        }
    }
}
