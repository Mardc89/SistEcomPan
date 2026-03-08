using Datos.Models;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;


namespace Datos.Interfaces
{
    public interface IPedidoNew:IGenericRepository<Pedidos>
    {
        public Task<(List<PedidoDTO>, int totalItems)> ObtenerPedidos(string searchTerm, int page, int itemsPerPage);
        Task<Pedidos> Registrar(Pedidos modelo,DataTable DataTable);
        Task<List<Pedidos>> Reporte(DateTime FechaInicio,DateTime FechaFin);
        Task<List<Pedidos>> ConsultarPedido(DateTime? fechaPedido=null);
        Task<List<Pedidos>> ConsultarTotalDePedidos(DateTime fechaPedido);
        Task<List<Pedidos>> BuscarTotal(string? codigo = null, string? estado = null, int? idCliente = null);

        Task <Pedidos>ActualizarDetallePedido(Pedidos modelo, DataTable DataTable);
        Task<Pedidos> Buscar(string? c = null, string? p = null, int? d = null);
        Task<List<Pedidos>> Consultar(string? Codigo = null, string? FechaInicio = null, string? FechaFin = null, int? IdPedido = null);

    }
}
