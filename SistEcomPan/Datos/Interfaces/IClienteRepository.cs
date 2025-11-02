using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IClienteRepository:IGenericRepository<Clientes>
    {
        Task<Clientes> Crear(Clientes modelo);
        Task<Clientes> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Clientes> Verificar(string? c = null, string? p = null, int? d = null);
  
    }
}
