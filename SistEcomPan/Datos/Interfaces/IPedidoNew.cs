using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IPedidoNew:IGenericRepository<Pedidos>
    {
        Task<Pedidos> Registrar(Pedidos modelo,DataTable DataTable);
        Task<List<Pedidos>> Reporte(DateTime FechaInicio,DateTime FechaFin);
    }
}
