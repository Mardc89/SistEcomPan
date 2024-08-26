using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDestinatarioMensajeService
    {
        Task<DestinatarioMensaje> Registrar(DestinatarioMensaje entidad);
        Task<DestinatarioMensaje> Editar(DestinatarioMensaje entidad);
        Task<bool> Eliminar(int IdMensaje);
        Task<List<DestinatarioMensaje>> Lista();
        Task<string> NombreDelDestinatario(int IdMensaje);
        Task<string> NombreUserDestinatario(int IdMensaje);
        Task<string> correoDestinatario(int IdMensaje);
        Task<int> IdDestinatario(string correo);
        Task<string> Destinatario(string correo);
    }
}
