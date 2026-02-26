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
        Task<Clientes> Buscar(string? c = null, string? p = null, int? d = null, string? a = null, string? n = null,string?dni=null);
        Task<Clientes> BuscarCliente(string? c = null);
        Task<Clientes> Verificar(string? c = null, string? p = null, int? d = null);

    }
}
