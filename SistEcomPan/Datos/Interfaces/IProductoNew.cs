using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IProductoNew :IGenericRepository<Productos>
    {
        Task<Productos> Crear(Productos modelo);
        Task<Dictionary<string, int>> ProductosTopUltimaSemana();
        Task<Dictionary<string, int>> MisProductosTopUltimaSemana(DateTime fechaInicio, int idCliente);
        Task<Productos> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Productos> Verificar(string? c = null, string? p = null, int? d = null);
    }
}
