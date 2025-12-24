using Entidades;
using Negocio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IPedidoService
    {
        //Task<List<Productos>> ObtenerProductos(string busqueda);
        Task<List<Pedidos>> MisPedidos(string searchTerm, DateTime? fechaBusquedaUtc, string busqueda = "");
        Task<(List<DetallePedidoBO> Items, int Total)> ObtenerDetalleFinal(string codigo);
        Task<Pedidos> Registrar(Pedidos entidad);
        Task <Pedidos>Actualizar(Pedidos entidad);
        Task<bool> Eliminar(int IdPedido);
        Task<List<Pedidos>> Historial(string numeroPedido, string fechaInicio, string fechaFin);
        Task<Pedidos> Detalle(string numeroPedido);
        Task<List<DetallePedido>> Reporte(string fechaInicio, string fechaFin);
        Task<List<Pedidos>> Lista();
        Task<int> ObtenerIdCliente(string Codigo);
        Task<string> NombreDelCliente(string Codigo);
        Task<IQueryable<Pedidos>> ObtenerNombre();

    }
}
