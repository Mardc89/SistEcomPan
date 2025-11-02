using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IProductoRepository:IGenericRepository<Productos>
    {
        Task<Productos> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Productos> Verificar(string? c = null, string? p = null, int? d = null);
        Task<List<Productos>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null);
    }
}
