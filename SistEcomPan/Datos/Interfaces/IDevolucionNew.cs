using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{ 
    public interface IDevolucionNew
    {
        Task<Devolucion> Registrar(Devolucion modelo, DataTable DataTable);
        Task<Devolucion> Buscar(string? c = null, string? p = null, int? d = null);
        Task<List<Devolucion>> Lista();
        Task<bool> Eliminar(int d);

    }
}
