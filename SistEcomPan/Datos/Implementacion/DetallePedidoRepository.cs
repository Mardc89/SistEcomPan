using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class DetallePedidoRepository : IGenericRepository<DetallePedido>
    {
        private readonly string _cadenaSQL = "";

        public DetallePedidoRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public Task<IQueryable<DetallePedido>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<DetallePedido> Crear(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(DetallePedido modelo)
        {
            throw new NotImplementedException();
        }

        public Task<List<DetallePedido>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<DetallePedido>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
