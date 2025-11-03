using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IPagoService
    {
        Task<Pagos> Registrar(Pagos entidad);
        Task<Pagos> Editar(Pagos entidad);
        Task<List<Pagos>> Lista();

        //Task<IQueryable<Pagos>> ObtenerNombre();
    }
}
