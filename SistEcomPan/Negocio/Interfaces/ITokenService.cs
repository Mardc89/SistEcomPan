using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface ITokenService
    {
        Task<bool> Crear(Tokens entidad);
        Task<Tokens> Editar(Tokens entidad);
        Task<Tokens> Buscar(string token);

    }
}
