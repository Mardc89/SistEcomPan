using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IPagoNew: IGenericRepository<Pagos>
    {
        Task<Pagos> Registrar(Pagos modelo, DataTable DataTable);
        Task<Pagos> Editar(Pagos modelo, DataTable DataTable);
        Task<List<Pagos>> ConsultarPagos(DateTime? fechaPago);
        Task<List<Pagos>> Reporte(DateTime FechaInicio, DateTime FechaFin);
        Task<List<Pagos>> ConsultarMisPagos(DateTime fechaPago, int idCliente);
    }
}
