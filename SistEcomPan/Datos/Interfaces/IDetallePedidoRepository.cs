using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IDetallePedidoRepository
    {
        Task<List<DetallePedido>> Lista();
        Task<DetallePedido> Buscar(string? c = null, string? p = null, int? d = null);
        Task<List<DetallePedido>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null);
    }
}
