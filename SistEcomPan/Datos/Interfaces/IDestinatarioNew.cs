using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IDestinatarioNew:IGenericRepository<Mensajes>
    {
        Task<Mensajes> Crear(Mensajes modelo, DestinatarioMensaje destino);
        Task<Mensajes> CrearRespuestaMensaje(Mensajes modelo, DestinatarioMensaje destino);
    }
}
