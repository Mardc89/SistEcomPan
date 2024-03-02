using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class DetallePedidoRepository : IGenericRepository<DetallePedido>
    {
        private readonly string _cadenaSQL = "";

        public DetallePedidoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public async Task<List<DetallePedido>> Lista()
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDetallePedidos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DetallePedido
                        {
                            IdDetallePedido = Convert.ToInt32(dr["IdDetallePedido"]),
                            IdPedido = Convert.ToInt32(dr["IdPedido"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
            }

            return lista;
        }

        public Task<IQueryable<DetallePedido>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<DetallePedido> Crear(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<DetallePedido>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
