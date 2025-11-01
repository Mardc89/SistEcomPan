using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IProductoNew 
    {
        Task<List<Productos>> Lista();
        Task<Productos> Crear(Productos modelo);
        Task<bool> Editar(Productos modelo);
        Task<bool> Eliminar(int d);
        Task<Dictionary<string, int>> ProductosTopUltimaSemana();
        Task<Dictionary<string, int>> MisProductosTopUltimaSemana(DateTime fechaInicio, int idCliente);
        Task<Productos> Buscar(string? c = null, string? p = null, int? d = null);
    }
}
