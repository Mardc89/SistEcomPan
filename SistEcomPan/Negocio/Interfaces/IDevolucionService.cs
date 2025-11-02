using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDevolucionService
    {
        Task<Devolucion> Registrar(Devolucion entidad);
        Task<bool> Eliminar(int IdDevolucion);
        Task<List<Devolucion>> Lista();
    }
}
