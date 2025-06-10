using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDashBoardService
    {
        Task<int> TotalPedidosUltimaSemana();
        Task<string> TotalIngresosDelDia();
        Task<int> TotalProductos();
        Task<int> TotalCategorias();
        Task<int> TotalDeLatas();
        Task<Dictionary<string,decimal?>>PedidosUltimaSemana();
        Task<Dictionary<string, int>> ProductosTopUltimaSemana();
    }
}
