using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IConfiguracionRepository<T> where T : class
    {
        Task<List<T>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null);
    }
}
