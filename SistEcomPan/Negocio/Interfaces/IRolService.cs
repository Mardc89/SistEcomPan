using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IRolService
    {
        Task<List<Roles>> lista();
        Task<IQueryable<Roles>> ObtenerNombre();
        Task<string>ConsultarRol(int? IdRol);
    }
}
