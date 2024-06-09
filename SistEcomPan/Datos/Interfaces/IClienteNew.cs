using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IClienteNew
    {
        Task<Clientes> ConsultarCliente(string? Correo=null,string?Clave=null,int? IdCliente=null);
        Task<Clientes> ConsultarCorreo(string? Correo = null, string? Clave = null, int? IdCliente = null);
    }
}
