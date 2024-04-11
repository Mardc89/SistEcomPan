using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IPagoContado
    {
        Task<Pagos> Pagando(Pagos modelo);
        Task<Pagos> Editando(Pagos modelo);
    }
}
