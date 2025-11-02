using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IDestinatarioMensajeRepository:IGenericRepository<DestinatarioMensaje>
    {
        Task<bool> Guardar(DestinatarioMensaje modelo);
        Task<DestinatarioMensaje> Crear(DestinatarioMensaje modelo);
        Task<DestinatarioMensaje> Buscar(string? c = null, string? p = null, int? d = null);
        Task<DestinatarioMensaje> Verificar(string? c = null, string? p = null, int? d = null);
    }
}
