using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<Clientes>> Lista();
        Task<Clientes> Crear(Clientes modelo);
        Task<bool> Editar(Clientes modelo);
        Task<bool> Eliminar(int d);

        Task<Clientes> Buscar(string? c = null, string? p = null, int? d = null);
        Task<Clientes> Verificar(string? c = null, string? p = null, int? d = null);
  
    }
}
