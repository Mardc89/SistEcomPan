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
        private readonly IDetallePedidoRepository _repositorioDetallePedido ;

        public DetallePedidoService(IDetallePedidoRepository repositorioDetallePedido)
        {
            _repositorioDetallePedido = repositorioDetallePedido;
                
        }

        public async Task<List<DetallePedido>> Lista()
        {
            List<DetallePedido> query = await _repositorioDetallePedido.Lista();
            return query;
        }

        public async Task<int> Buscar(int idPedido)
        {
            DetallePedido query = await _repositorioDetallePedido.Buscar(null,null,idPedido);
            return query.IdPedido;
        }

        public async Task<IQueryable<DetallePedido>> ObtenerNombre()
        {
            List<DetallePedido> lista = await _repositorioDetallePedido.Lista();
            return lista.AsQueryable();
        }


    }
}
