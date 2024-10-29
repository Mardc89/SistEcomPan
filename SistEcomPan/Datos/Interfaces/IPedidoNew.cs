using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Datos.Interfaces
{
    public interface IPedidoNew:IGenericRepository<Pedidos>
    {
        Task<Pedidos> Registrar(Pedidos modelo,DataTable DataTable);
        Task<List<Pedidos>> Reporte(DateTime FechaInicio,DateTime FechaFin);
        Task<List<Pedidos>> ConsultarPedido(DateTime? fechaPedido=null, int? idCliente = null);
        Task<List<Pedidos>> ConsultarTotalDePedidos(DateTime fechaPedido);
        Task<List<Pedidos>> BuscarTotal(string? codigo = null, string? estado = null, int? idCliente = null);

        Task <Pedidos>ActualizarDetallePedido(Pedidos modelo, DataTable DataTable);
    }
}
