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

namespace Negocio.Implementacion
{
    public class DashBoardServiceCliente : IDashBoardServiceCliente
    {
        private readonly IPedidoNew _repositorioPedidos;
        private readonly IGenericRepository<Pagos> _repositorioPagos;
        private readonly IGenericRepository<Clientes> _repositorioClientes;
        private readonly IGenericRepository<Mensajes> _repositorioMensajes;
        private readonly ProductoRepository _repositorioProductoTop;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardServiceCliente
        (
            ProductoRepository repositorioProductoTop,
            IPedidoNew repositorioPedidos,
            IGenericRepository<Pagos> repositorioPagos,
            IGenericRepository<Mensajes> repositorioMensajes,
            IGenericRepository<Clientes> repositorioClientes
        )
        {
            _repositorioProductoTop = repositorioProductoTop;
            _repositorioPedidos = repositorioPedidos;
            _repositorioMensajes = repositorioMensajes;
            _repositorioPagos = repositorioPagos;
            _repositorioClientes = repositorioClientes;
            FechaInicio = FechaInicio;

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
                List<Pedidos> query = await _repositorioPedidos.ConsultarPedido(FechaInicio.Date);
                decimal resultado = query.Select(x => x.MontoTotal).Sum(x => x.Value);

                return Convert.ToString(resultado, new CultureInfo("es-PE"));

            }
            catch
            {

                throw;
            }
        }



        public async Task<int> TotalDeMisPedidos(string correo)
        {
            try
            {
                Clientes cliente= await _repositorioClientes.Buscar(correo, null,null);
                var IdCliente = cliente.IdCliente;

                List<Pedidos> pedidos = await _repositorioPedidos.BuscarTotal(null, null,IdCliente);
                int total =pedidos.Count();

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

                int total = pedidosFiltrados.Count();

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


        public async Task<Dictionary<string, int>> PedidosUltimaSemana()
        {
            try
            {
                List<Pedidos> query = await _repositorioPedidos
                    .ConsultarPedido(FechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaPedido.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
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

            return await _repositorioProductoTop.ProductosTopUltimaSemana();

        }

    }
}
