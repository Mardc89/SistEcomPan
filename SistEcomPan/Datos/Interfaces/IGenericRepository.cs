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
        Task<bool> Editar(T modelo);
        Task<bool> Eliminar(int d);

    }
}
