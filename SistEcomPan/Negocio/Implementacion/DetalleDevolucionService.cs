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
    public class DetalleDevolucionService : IDetalleDevolucionService
    {
        private readonly IDetalleDevolucion _repositorioDetalleDevolucion;

        public DetalleDevolucionService(IDetalleDevolucion repositorioDetalleDevolucion)
        {
            _repositorioDetalleDevolucion = repositorioDetalleDevolucion;

        }

        public async Task<List<DetalleDevolucion>> Lista()
        {
            List<DetalleDevolucion> query = await _repositorioDetalleDevolucion.Lista();
            return query;
        }
    }
}
