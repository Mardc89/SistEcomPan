using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IDistritoRepository:IGenericRepository<Distritos>
    {
        Task<bool> Guardar(Distritos entidad);
        Task<Distritos> Crear(Distritos entidad);
        Task<Distritos> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Distritos> Verificar(string? c = null, string? p = null, int? d = null);

    }
}
