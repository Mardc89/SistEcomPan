using Entidades;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuarios> Crear(Usuarios entidad);
        Task<Usuarios> Editar(Usuarios entidad,Stream foto);
    }
}
