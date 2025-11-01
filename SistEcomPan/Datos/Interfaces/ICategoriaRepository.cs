using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface ICategoriaRepository:IGenericRepository<Categorias>
    {
        Task<Categorias> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Categorias> Verificar(string? c = null, string? p = null, int? d = null);

    }
}
