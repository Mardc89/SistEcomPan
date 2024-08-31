using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IMensajeService
    {
        Task<Mensajes> Registrar(Mensajes entidad,DestinatarioMensaje destino);
        Task<Mensajes> RegistrarMensaje(Mensajes entidad, DestinatarioMensaje destino);
        Task<Mensajes> Editar(Mensajes entidad);
        Task<bool> Eliminar(int IdMensaje);
        Task<List<Mensajes>> Lista();
        Task<string> NombreDelRemitente(string Tipo,int IdRemitente);
        Task<string> NombreDelDestinatario(string Tipo,int IdDestinataario);
        Task<string> correoDestinatario(string Tipo, int IdDestinataario);
        Task<string> correoRemitente(string Tipo, int IdRemitente);
        Task<string> NombreUserDestinatario(int IdMensaje);
        Task<int> IdDestinatario(string correo);
        Task<int> IdRemitente(string correo);
        Task<string> Destinatario(string correo);
        Task<string> Remitente(string correo);
      

    }
}
