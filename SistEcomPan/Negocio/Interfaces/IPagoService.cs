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
        Task<List<Productos>> ObtenerProductos(string busqueda);
        Task<Pagos> Registrar(Pagos entidad);
        Task<bool> Eliminar(int IdPago);
        Task<List<Pagos>> Historial(string numeroPago, string fechaInicio, string fechaFin);
        Task<Pagos> Detalle(string numeroPago);
        Task<List<DetallePago>> Reporte(string fechaInicio, string fechaFin);
        Task<List<Pagos>> Lista();
        Task<IQueryable<Pagos>> ObtenerNombre();
    }
}
