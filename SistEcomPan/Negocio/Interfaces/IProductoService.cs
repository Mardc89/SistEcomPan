using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IProductoService
    {
        Task<List<Productos>> Lista();
        Task<Productos> Crear(Productos entidad, Stream  Imagen= null, string NombreImagen = "");
        Task<Productos> Editar(Productos entidad, Stream Imagen= null, string NombreImagen = "");
        Task<bool> Eliminar(int IdProducto);
    }
}
