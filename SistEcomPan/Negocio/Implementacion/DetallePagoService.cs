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
    public class DetallePagoService:IDetallePagoService
    {
        private readonly IGenericRepository<DetallePago> _repositorioDetallePago;

        public DetallePagoService(IGenericRepository<DetallePago> repositorioDetallePago)
        {
            _repositorioDetallePago = repositorioDetallePago;

        }

        public async Task<List<DetallePago>> Lista()
        {
            List<DetallePago> query = await _repositorioDetallePago.Lista();
            return query;
        }

        public async Task<IQueryable<DetallePago>> ObtenerNombre()
        {
            List<DetallePago> lista = await _repositorioDetallePago.Lista();
            return lista.AsQueryable();
        }
    }
}
