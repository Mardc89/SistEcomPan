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
        Task<Devolucion> Actualizar(Devolucion entidad);
        Task<bool> Eliminar(int IdDevolucion);
        Task<Pedidos> Detalle(string numeroDevolucion);
        Task<List<Devolucion>> Lista();
    }
}
