using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> Lista();
        Task<List<T>> Consultar(string? c = null, string? p = null, string? d = null);
        Task<bool> Guardar(T modelo);
        Task<T> Crear(T modelo);
        Task<bool> Editar(T modelo);
        Task<bool> Eliminar(int d);
        Task<IQueryable<T>> Consultar();
        Task<T> Buscar(string?c = null,string?p=null,int?d = null);
        Task<T> Verificar(string?c=null,string?p=null,int?d=null);
        Task<IQueryable<T>> Obtener(string consulta);
    }
}
