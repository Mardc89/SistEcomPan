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
    public class PedidoNew : PedidoRepository, IPedidoEnvio
    {
        private readonly string _cadenaSQL = "";
        private readonly IGenericRepository<Productos> _repositorioProducto;
        private readonly IPedidoNew _repositorioPedido;
        private readonly IGenericRepository<Descuentos> _repositorioDescuento;
        private readonly IGenericRepository<NumeroDocumento> _repositorioNumDocumento;

        public PedidoNew(
            IGenericRepository<Productos> repositorioProducto, IPedidoNew repositorioPedido,
            IGenericRepository<Descuentos> repositorioDescuento, IGenericRepository<NumeroDocumento> repositorioNumDocumento,
            IConfiguration configuration): base(configuration){

            _repositorioProducto = repositorioProducto;
            _repositorioPedido = repositorioPedido;
            _repositorioDescuento = repositorioDescuento;
            _repositorioNumDocumento = repositorioNumDocumento;
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
            
        }

        public async Task<Pedidos> Registrando(Pedidos entidad)
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

    }
}
