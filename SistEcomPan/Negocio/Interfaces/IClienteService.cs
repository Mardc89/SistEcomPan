using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IClienteService
    {
        Task<List<Clientes>> Lista();
        Task<Clientes> Crear(Clientes entidad, Stream foto = null, string Nombrefoto = "", string UrlPlantillaCorreo = "");
        Task<Clientes> Editar(Clientes entidad, Stream foto = null, string Nombrefoto = "");
        Task<bool> Eliminar(int IdCliente);
        Task<Clientes> ObtenerPorCredenciales(string Correo, string Clave);
        Task<Clientes> ObtenerPorId(int IdCliente);
        Task<bool> GuardarPerfil(Clientes entidad, Stream foto = null, string Nombrefoto = "");
        Task<bool> CambiarClave(int IdCliente, string ClaveActual, string ClaveNueva);
        Task<bool> RestablecerClave(string ClaveNueva, string Correo, string UrlPlantillaCorreo);
        Task<IQueryable<Clientes>> ObtenerNombre();
        Task<string> ObtenerNombreCompleto(int IdCliente);
        Task<string> ObtenerCorreo(int IdCliente);
        Task<int> ObtenerIdCorreoCliente(string correo);
        Task<Clientes> ObtenerCliente(string correo);
        Task<string> ObtenerDni(int IdCliente);
    }
}
