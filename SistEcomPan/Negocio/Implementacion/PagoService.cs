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
    public class PagoService : IPagoService
    {

        private readonly IPagoContado _repositorioPagoContado;
        private readonly IGenericRepository<Pagos> _repositorioPago;

        public PagoService(IPagoContado repositorioPagoContado,
            IGenericRepository<Pagos> repositorioPago)
        {
            _repositorioPagoContado = repositorioPagoContado;
            _repositorioPago = repositorioPago;
        }

        public Task<Pagos> Detalle(string numeroPago)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdPedido)
        {
            throw new NotImplementedException();
        }

        public Task<List<Pagos>> Historial(string numeroPago, string fechaInicio, string fechaFin)
        {
            throw new NotImplementedException();
        }


        public async Task<Pagos> Registrar(Pagos entidad)
        {
            try
            {
                return await _repositorioPagoContado.Pagando(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Pagos> Editar(Pagos entidad)
        {
            try
            {
                return await _repositorioPagoContado.Editando(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<Pagos>> Lista()
        {
            List<Pagos> query = await _repositorioPago.Lista();
            return query;
        }

        public async Task<IQueryable<Pagos>> ObtenerNombre()
        {
            List<Pagos> lista = await _repositorioPago.Lista();
            return lista.AsQueryable();
        }


        public Task<List<Productos>> ObtenerProductos(string busqueda)
        {
            throw new NotImplementedException();
        }



        public Task<List<DetallePago>> Reporte(string fechaInicio, string fechaFin)
        {
            throw new NotImplementedException();
        }
    }
}
