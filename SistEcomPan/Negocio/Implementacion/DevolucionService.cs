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
    public class DevolucionService : IDevolucionService
    {
        private readonly IDevolucionProducto _repositorioDevolucionProducto;
        private readonly IGenericRepository<Devolucion> _repositorioDevolucion;

        public DevolucionService(IDevolucionProducto repositorioDevolucionProducto, IGenericRepository<Devolucion> repositorioDevolucion)
        {
            _repositorioDevolucionProducto = repositorioDevolucionProducto;
            _repositorioDevolucion = repositorioDevolucion;
        }
        public Task<Devolucion> Actualizar(Devolucion entidad)
        {
            throw new NotImplementedException();
        }

        public Task<Pedidos> Detalle(string numeroDevolucion)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdDevolucion)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Devolucion>> Lista()
        {
            List<Devolucion> query = await _repositorioDevolucion.Lista();
            return query;
        }

        public async Task<Devolucion> Registrar(Devolucion entidad)
        {
            try
            {
                return await _repositorioDevolucionProducto.Registrando(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
