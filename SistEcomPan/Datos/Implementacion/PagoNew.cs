using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class PagoNew : PagoRepository, IPagoContado
    {
        private readonly string _cadenaSQL = "";
        private readonly IPagoNew _repositorioPago;


        public PagoNew(IPagoNew repositorioPago,IConfiguration configuration) : base(configuration)
        {

            _repositorioPago = repositorioPago;
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<Pagos> Pagando(Pagos entidad)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();
                try
                {
                    SqlCommand cmd = conexion.CreateCommand();
                    cmd.Transaction = transaccion;
                    DataTable detallePago = new DataTable();
                    detallePago.Locale = new CultureInfo("es-Pe");
                    detallePago.Columns.Add("MontoAPagar", typeof(string));
                    detallePago.Columns.Add("PagoDelCliente", typeof(string));
                    detallePago.Columns.Add("DeudaDelCliente", typeof(string));
                    detallePago.Columns.Add("CambioDelCliente", typeof(string));

                    foreach (DetallePago detalle in entidad.DetallePago)
                    {
                        detallePago.Rows.Add(new object[] {
                        detalle.MontoAPagar,
                        detalle.PagoDelCliente,
                        detalle.DeudaDelCliente,
                        detalle.CambioDelCliente
                        });
                    }

                    transaccion.Commit();
                    return await _repositorioPago.Registrar(entidad, detallePago);
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        public async Task<Pagos> Editando(Pagos entidad)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();
                try
                {
                    SqlCommand cmd = conexion.CreateCommand();
                    cmd.Transaction = transaccion;
                    DataTable detallePago = new DataTable();
                    detallePago.Locale = new CultureInfo("es-Pe");
                    detallePago.Columns.Add("MontoAPagar", typeof(string));
                    detallePago.Columns.Add("PagoDelCliente", typeof(string));
                    detallePago.Columns.Add("DeudaDelCliente", typeof(string));
                    detallePago.Columns.Add("CambioDelCliente", typeof(string));

                    foreach (DetallePago detalle in entidad.DetallePago)
                    {
                        detallePago.Rows.Add(new object[] {
                        detalle.MontoAPagar,
                        detalle.PagoDelCliente,
                        detalle.DeudaDelCliente,
                        detalle.CambioDelCliente
                        });
                    }

                    transaccion.Commit();
                    return await _repositorioPago.Editar(entidad, detallePago);
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}
