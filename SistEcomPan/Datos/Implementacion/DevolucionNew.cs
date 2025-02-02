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
    public class DevolucionNew : DevolucionRepository, IDevolucionProducto
    {
        private readonly string _cadenaSQL = "";
        private readonly IDevolucionNew _repositorioDevolucion;
        private readonly IGenericRepository<NumeroDocumento> _repositorioNumDocumento;

        public DevolucionNew(
            IDevolucionNew repositorioDevolucion,
            IGenericRepository<NumeroDocumento> repositorioNumDocumento,
            IConfiguration configuration) : base(configuration)
        {

            _repositorioDevolucion = repositorioDevolucion;
            _repositorioNumDocumento = repositorioNumDocumento;
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }
        public async Task<Devolucion> Registrando(Devolucion entidad)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();
                try
                {
                    SqlCommand cmd = conexion.CreateCommand();
                    cmd.Transaction = transaccion;
                    DataTable detallePedido = new DataTable();
                    detallePedido.Locale = new CultureInfo("es-Pe");
                    detallePedido.Columns.Add("Categoria", typeof(string));
                    detallePedido.Columns.Add("Descripcion", typeof(string));
                    detallePedido.Columns.Add("Precio", typeof(decimal));
                    detallePedido.Columns.Add("CantidadPedido", typeof(int));
                    detallePedido.Columns.Add("Total", typeof(decimal));
                    detallePedido.Columns.Add("CantidadDevolucion", typeof(int));
                    foreach (DetalleDevolucion detalle in entidad.DetalleDevolucion)
                    {
                        detallePedido.Rows.Add(new object[] {
                        detalle.Categoria,
                        detalle.Descripcion,
                        detalle.Precio,
                        detalle.CantidadPedido,
                        detalle.Total,
                        detalle.CantidadDevolucion
                        });
                    }

                    //IQueryable<NumeroDocumento> buscarNumeroDocumento = await _repositorioNumDocumento.Consultar();
                    //IQueryable<NumeroDocumento> numerodocumentoEncontrado = buscarNumeroDocumento.Where(u => u.Gestion == "devoluciones");
                    //NumeroDocumento numeroDocumento = numerodocumentoEncontrado.First();

                    NumeroDocumento numeroDocumento = await _repositorioNumDocumento.Buscar("devoluciones",null,null);
                    numeroDocumento.UltimoNumero = numeroDocumento.UltimoNumero + 1;
                    await _repositorioNumDocumento.Editar(numeroDocumento);

                    string ceros = string.Concat(Enumerable.Repeat("0", numeroDocumento.CantidadDeDigitos));
                    string numeroDevolucion = ceros + numeroDocumento.UltimoNumero.ToString();
                    numeroDevolucion = numeroDevolucion.Substring(numeroDevolucion.Length - numeroDocumento.CantidadDeDigitos, numeroDocumento.CantidadDeDigitos);

                    entidad.CodigoDevolucion = numeroDevolucion;
                    transaccion.Commit();
                    return await _repositorioDevolucion.Registrar(entidad, detallePedido);
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
