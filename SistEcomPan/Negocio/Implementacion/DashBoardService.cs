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
    public class DashBoardService : IDashBoardService
    {
        private readonly IPedidoNew _repositorioPedidos;
        private readonly IGenericRepository<Categorias> _repositorioCategoria;
        private readonly IGenericRepository<Productos> _repositorioProducto;
        private readonly ProductoRepository _repositorioProductoTop;
        private DateTime FechaInicio=DateTime.Now;

        public DashBoardService
        (
            ProductoRepository repositorioProductoTop,
            IGenericRepository<Categorias> repositorioCategoria,
            IPedidoNew repositorioPedidos,
            IGenericRepository<Productos> repositorioProducto
        )
        {
            _repositorioProductoTop = repositorioProductoTop;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;
            _repositorioPedidos=repositorioPedidos;
            FechaInicio = FechaInicio.AddDays(-7);

        }        
        
        
        public async Task<int> TotalPedidosUltimaSemana()
        {
            try
            {
                IQueryable<Pedidos> query = await _repositorioPedidos.ConsultarPedido();
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
                IQueryable<Pedidos> query = await _repositorioPedidos.ConsultarPedido(FechaInicio.Date);
                decimal resultado = query
                    .Select(x => x.MontoTotal)
                    .Sum(v => v.Value);

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
                IQueryable<Productos> query = await _repositorioProducto.Consultar();
                int total = query.Count();

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
                IQueryable<Pedidos> query = await _repositorioPedidos
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

        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categorias> query = await _repositorioCategoria.Consultar();
                int total = query.Count();

                return total;

            }
            catch
            {

                throw;
            }
        }






    }
}
