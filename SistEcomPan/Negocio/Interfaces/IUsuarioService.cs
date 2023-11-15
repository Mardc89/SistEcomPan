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
        Task<Usuarios> Crear(Usuarios entidad,Stream Foto=null,string NombreFoto="",string UrlPlantillaCorreo="");
        

        
    }
}
