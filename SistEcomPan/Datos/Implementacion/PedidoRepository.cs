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
            Pedidos pedidos=new Pedidos();
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
                      
                        // Insertar la venta
                        string insertVentaQuery = "INSERT INTO Venta (FechaVenta) VALUES (@fechaVenta); SELECT SCOPE_IDENTITY()";
                            using (var insertVentaCommand = new SqlCommand(insertVentaQuery, conexion, transaction))
                            {
                                insertVentaCommand.Parameters.AddWithValue("@fechaVenta", DateTime.Now);
                                int IdPedido = Convert.ToInt32(insertVentaCommand.ExecuteScalar());

                                // Insertar el detalle de la venta
                                string insertDetalleQuery = "INSERT INTO DetallePedido (VentaID, ProductoID, Cantidad) VALUES (@pedidoId, @productoId, @cantidad)";
                                using (var insertDetalleCommand = new SqlCommand(insertDetalleQuery, conexion, transaction))
                                {

                                    insertDetalleCommand.Parameters.AddWithValue("@pedidoId", IdPedido);
                                    insertDetalleCommand.Parameters.AddWithValue("@productoId", entidad.IdPedido);
                                    insertDetalleCommand.Parameters.AddWithValue("@cantidad", entidad.IdPedido);

                                    insertDetalleCommand.ExecuteNonQuery();
                                }

                                // Actualizar el stock
                                string actualizarStockQuery = "UPDATE Producto SET CantidadDisponible = CantidadDisponible - @cantidad WHERE ProductoID = @productoId";
                                using (var actualizarStockCommand = new SqlCommand(actualizarStockQuery, conexion, transaction))
                                {
                                    actualizarStockCommand.Parameters.AddWithValue("@cantidad",2);
                                    actualizarStockCommand.Parameters.AddWithValue("@productoId",3);

                                    actualizarStockCommand.ExecuteNonQuery();
                                }
                            }

                            // Confirmar la transacción
                            transaction.Commit();
                            Console.WriteLine("Venta registrada con éxito.");
                        }

                    }
                    catch
                    {

                        // Algo salió mal, realizar un rollback para deshacer todas las operaciones
                        transaction.Rollback();
                        Console.WriteLine("Error al registrar la venta: ");

                    }

                    return pedidos;

                }


            }
        }

    public Task<List<Pedidos>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            throw new NotImplementedException();
        }
    }
}
