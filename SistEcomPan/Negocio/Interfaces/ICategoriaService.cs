using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categorias>> Lista();
        Task<Categorias> Crear(Categorias entidad);
        Task<Categorias> Editar(Categorias entidad);
        Task<bool> Eliminar(int IdCategoria);
        Task<IQueryable<Categorias>> ObtenerNombre();
    }
}
