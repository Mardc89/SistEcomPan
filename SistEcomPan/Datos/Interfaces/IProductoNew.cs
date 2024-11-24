using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IProductoNew : IGenericRepository<Productos>
    {
        Task<Dictionary<string, int>> ProductosTopUltimaSemana();
        Task<Dictionary<string, int>> MisProductosTopUltimaSemana(DateTime fechaInicio, int idCliente);
    }
}
