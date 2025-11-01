using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface INumeroDocumento
    {
        Task<NumeroDocumento> Buscar(string? c = null, string? p = null, int? d = null);
        Task<bool> Editar(NumeroDocumento modelo);

    }
}
