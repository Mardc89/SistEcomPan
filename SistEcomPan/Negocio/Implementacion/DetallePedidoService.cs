using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{ 
    public class DetallePedidoService : IDetallePedidoService
    {
        private readonly IGenericRepository<DetallePedido>_repositorioDetallePedido ;

        public DetallePedidoService(IGenericRepository<DetallePedido> repositorioDetallePedido)
        {
            _repositorioDetallePedido = repositorioDetallePedido;
                
        }

        public async Task<List<DetallePedido>> Lista()
        {
            List<DetallePedido> query = await _repositorioDetallePedido.Lista();
            return query;
        }

        public async Task<IQueryable<DetallePedido>> ObtenerNombre()
        {
            List<DetallePedido> lista = await _repositorioDetallePedido.Lista();
            return lista.AsQueryable();
        }


    }
}
