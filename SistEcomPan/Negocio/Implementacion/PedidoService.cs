using Datos.Interfaces;
using Entidades;
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
        private readonly IPedidoRepository _repositorioPedido;

        public PedidoService(IGenericRepository<Productos> repositorioProducto, IPedidoRepository repositorioPedido)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioPedido = repositorioPedido;
                
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
                decimal total = 0;
                DataTable detallePedido = new DataTable();
                detallePedido.Locale = new CultureInfo("es-Pe");
                detallePedido.Columns.Add("IdProducto", typeof(string));
                detallePedido.Columns.Add("Cantidad", typeof(int));
                detallePedido.Columns.Add("Total", typeof(decimal));

                foreach (DetallePedido detalle in entidad.DetallePedido){
                    decimal subtotal = Convert.ToDecimal(detalle.Cantidad.ToString()) * detalle.Producto.Precio;
                    total+= subtotal;

                    IQueryable<DetallePedido> buscarProducto = await _repositorioPedido.Consultar();
                    IQueryable<DetallePedido> productoEncontrado = buscarProducto.Where(u => u.IdProducto==detalle.IdProducto);
                    DetallePedido productoEditar = productoEncontrado.First();
                    productoEditar.Producto.Stock = productoEditar.Producto.Stock - detalle.Cantidad;

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
