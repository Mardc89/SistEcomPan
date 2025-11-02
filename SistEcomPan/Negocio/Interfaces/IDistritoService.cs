using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDistritoService
    {
        Task<List<Distritos>> Lista();
        Task<Distritos> Crear(Distritos entidad);
        Task<Distritos> Editar(Distritos entidad);
        Task<bool> Eliminar(int IdCategoria);
        Task<string> ConsultarDistrito(int? IdDistrito);
    }
}
