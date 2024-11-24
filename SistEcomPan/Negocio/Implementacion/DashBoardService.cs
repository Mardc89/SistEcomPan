using Datos.Implementacion;
using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Negocio.Implementacion
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IPedidoNew _repositorioPedidos;
        private readonly IPagoNew _repositorioPagos;
        private readonly IDetallePagoNew _repositorioDetallePagos;
        private readonly IProductoNew _repositorioProducto;
        private readonly IGenericRepository<Categorias> _repositorioCategoria;
        private readonly IGenericRepository<DetallePedido> _repositorioDetallePedido;
        private readonly IGenericRepository<Clientes> _repositorioClientes;
        private readonly IGenericRepository<Mensajes> _repositorioMensajes;
        private DateTime FechaInicio=DateTime.Now;

        public DashBoardService
        (
            IGenericRepository<Categorias> repositorioCategoria,
            IPedidoNew repositorioPedidos,
            IProductoNew repositorioProducto,
            IGenericRepository<Mensajes> repositorioMensajes,
            IGenericRepository<Clientes> repositorioClientes,
            IGenericRepository<DetallePedido> repositorioDetallePedido,
            IPagoNew repositorioPagos,
            IDetallePagoNew repositorioDetallePagos
        )
        {
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;
            _repositorioPedidos=repositorioPedidos;
            _repositorioMensajes = repositorioMensajes;
            _repositorioClientes = repositorioClientes;
            _repositorioDetallePedido = repositorioDetallePedido;
            _repositorioPagos = repositorioPagos;
            _repositorioDetallePagos = repositorioDetallePagos;
            FechaInicio = FechaInicio.AddDays(-7);

        }        
        
        
        public async Task<int> TotalPedidosUltimaSemana()
        {
            try
            {
                List<Pedidos> query = await _repositorioPedidos.ConsultarPedido(FechaInicio);
                int total = query.Count();
                return total;

            }
            catch
            {

                throw;
            }
           
        }       
        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                List<DetallePago> query = await _repositorioDetallePagos.ConsultarDetallePagos(FechaInicio);
                decimal resultado = query.Select(x => x.MontoAPagar).Sum(x=>x.Value);

                return Convert.ToString(resultado, new CultureInfo("es-PE"));

            }
            catch
            {

                throw;
            }
        }        
        
        public async Task<int> TotalProductos()
        {
            try
            {
                List<Productos> query = await _repositorioProducto.Lista();
                int total = query.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }

        public async Task<int> TotalDeMisPedidos(int idCliente)
        {
            try
            {
                List<Pedidos> query = await _repositorioPedidos.BuscarTotal(null,null,idCliente);
                int total = query.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }

        public async Task<int> TotalDeMisPagos(string DniPersonal)
        {
            try
            {
                var Pagolista = await _repositorioPagos.Lista();
                var clientelista = await _repositorioClientes.Lista();
                var PedidosLista = await _repositorioPedidos.Lista();
                var idCliente = clientelista.Where(x => x.Dni == DniPersonal).Select(x => x.IdCliente).FirstOrDefault();

                //var estadoCliente = Pedidolista.Where(x => x.IdCliente == idCliente).Select(x => x.Estado).FirstOrDefault();
                var idPedido = PedidosLista.Where(x => x.IdCliente == idCliente).Select(x => x.IdPedido).ToList();

                //var PagosPedidos = PedidosLista.Where(x => x.IdPedido == idPedido[i])

                // Filtro de búsqueda por término de búsqueda (searchTerm)
                var pedidosFiltrados = Pagolista.Where(p => idPedido.Contains(p.IdPedido)).ToList();

                int total =pedidosFiltrados.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }


        public async Task<int> TotalDeMisMensajes(string DniPersonal)
        {
            try
            {
                var MensajesLista = await _repositorioMensajes.Lista();
                var clientelista = await _repositorioClientes.Lista();
                //var PedidosLista = await _repositorioPedidos.Lista();
                var idCliente = clientelista.Where(x => x.Dni == DniPersonal).Select(x => x.IdCliente).ToList();

                //var estadoCliente = Pedidolista.Where(x => x.IdCliente == idCliente).Select(x => x.Estado).FirstOrDefault();
                //var ClienteID = PedidosLista.Where(x => x.IdCliente == idCliente).Select(x => x.IdCliente).ToList();

                //var PagosPedidos = PedidosLista.Where(x => x.IdPedido == idPedido[i])

                // Filtro de búsqueda por término de búsqueda (searchTerm)
                var mensajesFiltrados = MensajesLista.Where(p => idCliente.Contains(p.IdRemitente)).ToList();

                int total = mensajesFiltrados.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }


        public async Task<Dictionary<string, decimal?>> PedidosUltimaSemana()
        {
            try
            {
                List<Pedidos> query = await _repositorioPedidos
                    .ConsultarPedido(FechaInicio.Date);

                Dictionary<string, decimal?> resultado = query
                    .GroupBy(v => v.FechaPedido.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Sum(x=>x.MontoTotal) })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return resultado;

            }
            catch
            {

                throw;
            }
        }

        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {

            return await _repositorioProducto.ProductosTopUltimaSemana();

        }

        public async Task<int> TotalCategorias()
        {
            try
            {
                List<Categorias> query = await _repositorioCategoria.Lista();
                int total = query.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }

        public async Task<int> TotalDeLatas()
        {
            try
            {
                List<DetallePedido> detallePedidos = new List<DetallePedido>();
                List<DetallePedido> detallePedidosFinal = new List<DetallePedido>();
                List<Pedidos> query = await _repositorioPedidos.ConsultarTotalDePedidos(FechaInicio.Date);
                var ListPedidos = query.Select(x => x.IdPedido).ToList();
              
                foreach (var elemento in ListPedidos)
                {
                    var detalles = await _repositorioDetallePedido.Consultar(null, null, null,elemento);
                    detallePedidos.AddRange(detalles);
                }

                var Productos = await _repositorioProducto.Lista();
                var ProductosLatas=Productos.Where(x=>x.Descripcion.Contains("Lata de Pan")).Select(x=>x.IdProducto).ToList();

                foreach (var elemento in ProductosLatas)
                {
                    var detallefinal = detallePedidos.Where(x => x.IdProducto.Equals(elemento)).ToList();
                    if (detallefinal!=null)
                    {
                        detallePedidosFinal.AddRange(detallefinal);
                    }
                }

                int suma = detallePedidosFinal.Sum(x=>x.Cantidad);
                   

                return suma;

            }
            catch
            {

                throw;
            }
        }
    }
}
