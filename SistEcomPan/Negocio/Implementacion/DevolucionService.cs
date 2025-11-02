using Datos.Implementacion;
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
        private readonly IDevolucionNew _repositorioDevolucion;

        public DevolucionService(IDevolucionProducto repositorioDevolucionProducto, IDevolucionNew repositorioDevolucion)
        {
            _repositorioDevolucionProducto = repositorioDevolucionProducto;
            _repositorioDevolucion = repositorioDevolucion;
        }


        public async Task<bool> Eliminar(int IdDevolucion)
        {
            try
            {
                Devolucion DevolucionEncontrada = await _repositorioDevolucion.Buscar(null, null, IdDevolucion);
                if (DevolucionEncontrada == null)
                    throw new TaskCanceledException("La Devolucion no Existe");

                bool respuesta = await _repositorioDevolucion.Eliminar(DevolucionEncontrada.IdDevolucion);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
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
