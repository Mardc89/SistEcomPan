using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Interfaces;

namespace Datos.Implementacion
{
    public class DetalleDevolucionRepository : IGenericRepository<DetalleDevolucion>
    {
        private readonly string _cadenaSQL = "";

        public DetalleDevolucionRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");
        }

        public Task<DetalleDevolucion> Buscar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<DetalleDevolucion>> Consultar(string? c = null, string? p = null, string? m = null, int? d = null)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<DetalleDevolucion>> Consultar()
        {
            throw new NotImplementedException();
        }

        public Task<DetalleDevolucion> Crear(DetalleDevolucion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(DetalleDevolucion modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(DetalleDevolucion modelo)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DetalleDevolucion>> Lista()
        {
            try
            {
                List<DetalleDevolucion> lista = new List<DetalleDevolucion>();
                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SPListaDetalleDevolucion", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(new DetalleDevolucion
                            {
                                IdDetalleDevolucion = Convert.ToInt32(dr["IdDetalleDevolucion"]),
                                IdDevolucion = Convert.ToInt32(dr["IdDevolucion"]),
                                Categoria = dr["Categoria"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                CantidadPedido = Convert.ToInt32(dr["CantidadPedido"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                CantidadDevolucion = Convert.ToInt32(dr["CantidadDevolucion"]),
                            });
                        }
                    }
                }

                return lista;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IQueryable<DetalleDevolucion>> Obtener(string consulta)
        {
            throw new NotImplementedException();
        }

        public Task<DetalleDevolucion> Verificar(string? c = null, string? p = null, int? d = null)
        {
            throw new NotImplementedException();
        }
    }
}
