using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IRolRepository:IGenericRepository<Roles>
    {
        Task<Roles> Crear(Roles modelo);
        Task<Roles> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Roles> Verificar(string? c = null, string? p = null, int? d = null);
    }
}
