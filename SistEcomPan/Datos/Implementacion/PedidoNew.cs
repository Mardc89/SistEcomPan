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
        private readonly IGenericRepository<NumeroDocumento> _repositorioNumDocumento;

        public PedidoNew(
            IGenericRepository<Productos> repositorioProducto, IPedidoNew repositorioPedido,
            IGenericRepository<NumeroDocumento> repositorioNumDocumento,
            IConfiguration configuration): base(configuration){

            _repositorioProducto = repositorioProducto;
            _repositorioPedido = repositorioPedido;
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


                        IQueryable<Productos> producto = await _repositorioProducto.Consultar();
                        IQueryable<Productos> productoEncontrado = producto.Where(u => u.IdProducto == detalle.IdProducto);
                        var PrecioProducto = productoEncontrado.First().Precio;

                        subtotal = Convert.ToDecimal(detalle.Cantidad.ToString()) *PrecioProducto;
                        total += subtotal;

                        IQueryable<Productos> buscarProducto = await _repositorioProducto.Consultar();
                        IQueryable<Productos> productoStock = buscarProducto.Where(u => u.IdProducto == detalle.IdProducto);
                        Productos productoEditar = productoStock.First();
                        productoEditar.Stock = productoEditar.Stock - detalle.Cantidad;
                        await _repositorioProducto.Editar(productoEditar);

                        detallePedido.Rows.Add(new object[] {
                        detalle.IdProducto,
                        detalle.Cantidad,
                        subtotal
                        });
                    }

                        IQueryable<NumeroDocumento> buscarNumeroDocumento = await _repositorioNumDocumento.Consultar();
                        IQueryable<NumeroDocumento> numerodocumentoEncontrado = buscarNumeroDocumento.Where(u => u.Gestion == "pedidos");
                        NumeroDocumento numeroDocumento = numerodocumentoEncontrado.First();


                        numeroDocumento.UltimoNumero = numeroDocumento.UltimoNumero + 1;
                        await _repositorioNumDocumento.Editar(numeroDocumento);

                        string ceros = string.Concat(Enumerable.Repeat("0", numeroDocumento.CantidadDeDigitos));
                        string numeroPedido = ceros + numeroDocumento.UltimoNumero.ToString();
                        numeroPedido = numeroPedido.Substring(numeroPedido.Length - numeroDocumento.CantidadDeDigitos, numeroDocumento.CantidadDeDigitos);

                        entidad.Codigo = numeroPedido;
                        
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
