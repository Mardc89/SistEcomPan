using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IUsuarioRepository:IGenericRepository<Usuarios>
    {
        Task<Usuarios> Buscar(string?c = null,string?p=null,int?d = null);
        Task<Usuarios> Verificar(string?c=null,string?p=null,int?d=null);

    }
}
