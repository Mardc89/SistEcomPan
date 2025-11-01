using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface ITokenRepository<Tokens>
    {
        Task<bool> Guardar(Tokens modelo);
        Task<Tokens> Buscar(string? c = null, string? p = null, int? d = null);
        Task<bool> Editar(Tokens modelo);
    }
}
