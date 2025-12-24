using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using Negocio.Interfaces;
using Negocio.Models;
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
        private readonly IPedidoEnvio _repositorioPedidoEnvio;
        private readonly IPedidoNew _repositorioPedido;
        private readonly IPagoNew _repositorioPago;
        private readonly IClienteService _clienteService;
        private readonly IProductoService _productoService;
        private readonly IDetallePedidoService _detallePedidoService;

        public PedidoService(IPedidoEnvio repositorioPedidoEnvio,IPedidoNew repositorioPedido, IPagoNew repositorioPago, IClienteService clienteService, IProductoService productoService, IDetallePedidoService detallePedidoService)
        {
            _repositorioPedidoEnvio = repositorioPedidoEnvio;
            _repositorioPedido = repositorioPedido;
            _repositorioPago = repositorioPago;
            _clienteService = clienteService;
            _productoService = productoService;
            _detallePedidoService = detallePedidoService;
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

        //public async Task<List<Productos>> ObtenerProductos(string busqueda)
        //{
        //    IQueryable<Productos> productos = await _repositorioProducto.Consultar();
        //    IQueryable<Productos> productosEvaluado = productos.Where(u => u.Estado ==true && string.Concat(u.IdProducto,u.Descripcion).Contains(busqueda));

        //    return productosEvaluado.ToList();
        //}

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

        public async Task<int> ObtenerIdCliente(string Codigo)
        {
            Pedidos pedido = await _repositorioPedido.Buscar(Codigo, null,null);
            return pedido.IdCliente;
        }

        public async Task<List<Pedidos>> MisPedidos(string searchTerm, DateTime? fechaBusquedaUtc, string busqueda = "")
        {
            var Pedidolista = await _repositorioPedido.Lista();
            var clientelista = await _clienteService.Lista();

            var idCliente = clientelista
                .Where(x => x.Dni == searchTerm)
                .Select(x => x.IdCliente)
                .FirstOrDefault();
            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pedidolista.Where(p => p.IdCliente.Equals(idCliente));


            var MisPedidos = pedidosFiltrados.Where(p =>
            string.IsNullOrWhiteSpace(busqueda) || p.Estado.ToLower().Contains(busqueda.ToLower()) || (
                fechaBusquedaUtc.HasValue && p.FechaPedido.HasValue
                    && p.FechaPedido.Value >= fechaBusquedaUtc.Value.Date
                    && p.FechaPedido.Value < fechaBusquedaUtc.Value.Date.AddDays(1)
            )
            );

            return MisPedidos.ToList();
        }

        public async Task<string> NombreDelCliente(string searchTerm)
        {
            var clientelista = await _clienteService.Lista();
            var Pedidolista = await _repositorioPedido.Lista();

            var idCliente = clientelista
                .Where(x => x.Dni == searchTerm)
                .Select(x => x.IdCliente)
                .FirstOrDefault();
            // Filtro de búsqueda por término de búsqueda (searchTerm)
            var pedidosFiltrados = Pedidolista.Where(p => p.IdCliente.Equals(idCliente));

            var clientePedido = pedidosFiltrados.FirstOrDefault().IdCliente;

            //var clientes = await _clienteService.ObtenerNombre();
            var clienteEncontrado = await _clienteService.ObtenerNombreCompleto(clientePedido);

            return clienteEncontrado;
        }


        public async Task<(List<DetallePedidoBO> Items, int Total)>ObtenerDetalleFinal(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return (new(), 0);

            var pedido = (await Lista())
                .FirstOrDefault(p => p.Codigo == codigo);

            if (pedido == null)
                return (new(), 0);

            var detalles = (await _detallePedidoService.Lista())
                .Where(d => d.IdPedido == pedido.IdPedido);

            var productos = (await _productoService.ObtenerNombre())
                .ToDictionary(p => p.IdProducto);

            var resultado = detalles
                .GroupBy(d => d.IdProducto)
                .Select(g =>
                {
                    var producto = productos[g.Key];

                    return new DetallePedidoBO
                    {
                        IdProducto = g.Key,
                        IdCategoria = producto.IdCategoria,
                        DescripcionProducto = producto.Descripcion,
                        Precio = producto.Precio,
                        CantidadTotal = g.Sum(x => x.Cantidad),
                        Total = g.Sum(x => x.Total)
                    };
                })
                .ToList();

            return (resultado, resultado.Count);
        }


    }
}
