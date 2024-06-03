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
    public class DetallePagoRepository : IGenericRepository<DetallePago>
    {
        private readonly string _cadenaSQL = "";

        public DetallePagoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }
        public Task<IQueryable<DetallePago>> Consultar()
        {
            throw new NotImplementedException();
        }

        public async Task<List<DetallePago>> Lista()
        {
            List<DetallePago> lista = new List<DetallePago>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaDetallePagos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new DetallePago
                        {
                            IdDetallePago = Convert.ToInt32(dr["IdDetallePago"]),
                            IdPago = Convert.ToInt32(dr["IdPago"]),
                            MontoAPagar= Convert.ToDecimal(dr["MontoAPagar"]),
                            PagoDelCliente = Convert.ToDecimal(dr["PagoDelCliente"]),
                            DeudaDelCliente  = Convert.ToDecimal(dr["DeudaDelCliente"]),
                            CambioDelCliente = Convert.ToDecimal(dr["CambioDelCliente"])
                        });
                    }
                }
            }

            return lista;
        }

        public Task<DetallePago> Crear(DetallePago modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(DetallePago modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(DetallePago modelo)
        {
            throw new NotImplementedException();
        }


        public Task<IQueryable<DetallePago>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
