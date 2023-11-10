using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuarios> Crear(Usuarios entidad,Stream foto);
        Task<Usuarios> Editar(Usuarios entidad,Stream foto);
    }
}
