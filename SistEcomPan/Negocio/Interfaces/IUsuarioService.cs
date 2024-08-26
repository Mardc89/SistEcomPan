using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuarios>> Lista();
        Task<Usuarios> Crear(Usuarios entidad, Stream foto = null, string Nombrefoto = "", string UrlPlantillaCorreo = "");
        Task<Usuarios> Editar(Usuarios entidad, Stream foto = null, string Nombrefoto = "");
        Task<bool> Eliminar(int IdUsuario);
        Task<Usuarios> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuarios> ObtenerPorId(int IdUsuario);
        Task<bool> GuardarPerfil(Usuarios entidad, Stream foto = null, string Nombrefoto = "");
        Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva);
        Task<bool> RestablecerClave(string claveNueva, string correo, string UrlPlantillaCorreo);
        Task<int> ObtenerIdCorreoUsuario(string correo);
        Task<string> ObtenerNombreCompleto(int IdUsuario);
        Task<string> ObtenerCorreo(int IdUsuario);
        Task<int> IdDestinatario(string correo);
        Task<int> IdRemitente(string correo);


    }
}
