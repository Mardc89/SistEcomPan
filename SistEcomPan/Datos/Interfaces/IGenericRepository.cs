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
        Task<bool> Guardar(T modelo);
        Task<bool> Editar(T modelo);
        Task<bool> Delete(int d);
        Task<IQueryable<T>> Consultar(Expression<Func<T, bool>> filtro = null);
    }
}
