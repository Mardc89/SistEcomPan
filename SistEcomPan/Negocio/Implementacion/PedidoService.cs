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
        private readonly IPedidoEnvio _repositorioPedidoEnvio;
        private readonly IGenericRepository<Pedidos> _repositorioPedido;
        private readonly IGenericRepository<Pagos> _repositorioPago;

        public PedidoService(IGenericRepository<Productos> repositorioProducto,IPedidoEnvio repositorioPedidoEnvio,
            IGenericRepository<Pedidos> repositorioPedido, IGenericRepository<Pagos> repositorioPago)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioPedidoEnvio = repositorioPedidoEnvio;
            _repositorioPedido = repositorioPedido;
            _repositorioPago = repositorioPago;
        }
        public Task<Pedidos> Detalle(string numeroPedido)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Pedidos>> Historial(string numeroPedido, string fechaInicio, string fechaFin)
        {
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;


            if (fechaInicio != "" && fechaFin != "")
            {
                List<Pedidos> query = await _repositorioPedido.Consultar(null, fechaInicio, fechaFin);
                return query;
            }
            else
            {
                List<Pedidos> query = await _repositorioPedido.Consultar(numeroPedido, null, null);
                return query;
            }


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
                return await _repositorioPedidoEnvio.Registrando(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Pedidos> Actualizar(Pedidos detalle)
        {
            try
            {
                 return await _repositorioPedidoEnvio.Actualizando(detalle);
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

        public async Task<List<Pedidos>> Lista()
        {
            List<Pedidos> query = await _repositorioPedido.Lista();
            return query;
        }

        public async Task<IQueryable<Pedidos>> ObtenerNombre()
        {
            List<Pedidos> lista = await _repositorioPedido.Lista();
            return lista.AsQueryable();
        }

        public async Task<bool> Eliminar(int IdPedido)
        {
            try
            {
                Pedidos pedidoEncontrado = await _repositorioPedido.Buscar(null, null, IdPedido);
                Pagos   pagoEncontrado = await _repositorioPago.Buscar(null, null, pedidoEncontrado.IdPedido);           
                if (pedidoEncontrado == null)
                    throw new ArgumentException("El Pedido no Existe");   
                if (pagoEncontrado == null)
                    throw new ArgumentException("El Pago no Existe");
                if (pagoEncontrado.MontoDeuda>=0)        
                    throw new InvalidOperationException("No se puede eliminar");
                if (pagoEncontrado.MontoDeuda==pagoEncontrado.MontoDePedido && pagoEncontrado.Estado.Equals("Pendiente")) {
                    return await _repositorioPedido.Eliminar(pedidoEncontrado.IdPedido);
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
