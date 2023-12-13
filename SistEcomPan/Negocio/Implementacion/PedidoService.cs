using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class PedidoService :IPedidoService
    {
        private readonly IGenericRepository<Productos> _repositorioProducto;
        private readonly IPedidoNew _repositorioPedido;
        private readonly IGenericRepository<Descuentos> _repositorioDescuento;
        private readonly IGenericRepository<NumeroDocumento> _repositorioNumDocumento;
        private readonly string _cadenaSQL = "";

        public PedidoService(IGenericRepository<Productos> repositorioProducto, IPedidoNew repositorioPedido,
            IGenericRepository<Descuentos> repositorioDescuento, IGenericRepository<NumeroDocumento> repositorioNumDocumento,
            IConfiguration configuration)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioPedido = repositorioPedido;
            _repositorioDescuento = repositorioDescuento;
            _repositorioNumDocumento = repositorioNumDocumento; 
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }
        public Task<Pedidos> Detalle(string numeroPedido)
        {
            throw new NotImplementedException();
        }

        public Task<List<Pedidos>> Historial(string numeroPedido, string fechaInicio, string fechaFin)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Productos>> ObtenerProductos(string busqueda)
        {
            IQueryable<Productos> productos = await _repositorioProducto.Consultar();
            IQueryable<Productos> productosEvaluado = productos.Where(u => u.Estado ==true && string.Concat(u.IdProducto,u.Descripcion).Contains(busqueda));

            return productosEvaluado.ToList();
        }

        public async Task<Pedidos> Registrar(Pedidos entidad)
        {
           
            try
            {
                decimal total = 0,subtotal=0;
                DataTable detallePedido = new DataTable();
                detallePedido.Locale = new CultureInfo("es-Pe");
                detallePedido.Columns.Add("IdProducto", typeof(string));
                detallePedido.Columns.Add("Cantidad", typeof(int));
                detallePedido.Columns.Add("Total", typeof(decimal));

                foreach (DetallePedido detalle in entidad.DetallePedido){

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
                    IQueryable<Productos> productoEncontrado = buscarProducto.Where(u => u.IdProducto==detalle.IdProducto);
                    Productos productoEditar = productoEncontrado.First();
                    productoEditar.Stock = productoEditar.Stock - detalle.Cantidad;
                    await _repositorioProducto.Editar(productoEditar);

                    IQueryable<NumeroDocumento> buscarNumeroDocumento = await _repositorioNumDocumento.Consultar();
                    IQueryable<NumeroDocumento> numerodocumentoEncontrado = buscarNumeroDocumento.Where(u => u.Gestion =="Pedidos");
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

                return await _repositorioPedido.Registrar(entidad,detallePedido);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<DetallePedido>> Reporte(string fechaInicio, string fechaFin)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Pedidos>> lista()
        {
            List<Pedidos> query = await _repositorioPedido.Lista();
            return query;
        }

        public async Task<IQueryable<Pedidos>> ObtenerNombre()
        {
            List<Pedidos> lista = await _repositorioPedido.Lista();
            return lista.AsQueryable();
        }

        public Task<bool> Eliminar(int IdPedido)
        {
            throw new NotImplementedException();
        }
    }
}
