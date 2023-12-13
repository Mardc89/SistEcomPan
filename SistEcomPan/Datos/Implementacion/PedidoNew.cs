using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class PedidoNew : PedidoRepository, IPedidoNew
    {
        private readonly string _cadenaSQL = "";
        public PedidoNew(IConfiguration configuration) : base(configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public Task<Pedidos> Registrar(Pedidos modelo, DataTable DataTable)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlTransaction transaccion= conexion.BeginTransaction();
                try
                {
                    SqlCommand cmd = conexion.CreateCommand();
                    cmd.Transaction = transaccion;
                    decimal total = 0, subtotal = 0;
                    DataTable detallePedido = new DataTable();
                    detallePedido.Locale = new CultureInfo("es-Pe");
                    detallePedido.Columns.Add("IdProducto", typeof(string));
                    detallePedido.Columns.Add("Cantidad", typeof(int));
                    detallePedido.Columns.Add("Total", typeof(decimal));

                    foreach (DetallePedido detalle in entidad.DetallePedido)
                    {

                        IQueryable<Descuentos> descuento = await _repositorioDescuento.Consultar();
                        IQueryable<Descuentos> descuentoEncontrado = descuento.Where(u => u.IdProducto == detalle.IdProducto);
                        Descuentos descuentoProducto = descuentoEncontrado.First();

                        bool estado = Convert.ToBoolean(descuentoProducto.Estado);

                        if (estado)
                        {
                            subtotal = Convert.ToDecimal(detalle.Cantidad.ToString()) * detalle.Producto.Precio - descuentoProducto.Descuento;
                            total += subtotal;

                        }
                        else
                        {
                            subtotal = Convert.ToDecimal(detalle.Cantidad.ToString()) * detalle.Producto.Precio;
                            total += subtotal;
                        }
                        IQueryable<Productos> buscarProducto = await _repositorioProducto.Consultar();
                        IQueryable<Productos> productoEncontrado = buscarProducto.Where(u => u.IdProducto == detalle.IdProducto);
                        Productos productoEditar = productoEncontrado.First();
                        productoEditar.Stock = productoEditar.Stock - detalle.Cantidad;
                        await _repositorioProducto.Editar(productoEditar);

                        IQueryable<NumeroDocumento> buscarNumeroDocumento = await _repositorioNumDocumento.Consultar();
                        IQueryable<NumeroDocumento> numerodocumentoEncontrado = buscarNumeroDocumento.Where(u => u.Gestion == "Pedidos");
                        NumeroDocumento numeroDocumento = numerodocumentoEncontrado.First();


                        numeroDocumento.UltimoNumero = numeroDocumento.UltimoNumero + 1;
                        numeroDocumento.FechaActualizacion = DateTime.Now;
                        await _repositorioNumDocumento.Editar(numeroDocumento);

                        string ceros = string.Concat(Enumerable.Repeat("0", numeroDocumento.CantidadDeDigitos));
                        string numeroPedido = ceros + numeroDocumento.UltimoNumero.ToString();
                        numeroPedido = numeroPedido.Substring(numeroPedido.Length - numeroDocumento.CantidadDeDigitos, numeroDocumento.CantidadDeDigitos);

                        entidad.Codigo = numeroPedido;


                        detallePedido.Rows.Add(new object[] {
                        detalle.Producto.IdProducto,
                        detalle.Cantidad,
                        subtotal
                    });

                    }
                    entidad.MontoTotal = total;
                    transaccion.Commit();
                    return await _repositorioPedido.Registrar(entidad, detallePedido);
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        public Task<List<Pedidos>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            throw new NotImplementedException();
        }
    }
}
