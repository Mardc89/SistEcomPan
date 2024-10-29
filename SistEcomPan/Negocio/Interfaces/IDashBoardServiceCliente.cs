using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDashBoardServiceCliente
    {
        Task<int> TotalPedidosUltimaSemana();
        Task<string> TotalIngresosUltimaSemana();
        Task<int> TotalDeMisPagos(string dni);
        Task<int> TotalDeMisPedidos(string correo);
        Task<int> TotalDeMisMensajes(string dni);
        Task<Dictionary<string, decimal?>> PedidosUltimaSemana(string dni);
        Task<Dictionary<string, int>> ProductosTopUltimaSemana(string dni);
    }
}
