using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<bool> Registrar(Pedidos entidad, DataTable dataTable)
        {
            try
            {
                return await _repositorioPedido.Registrar(entidad,dataTable);
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
    }
}
