using Datos.Interfaces;
using Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Implementacion
{
    public class CategoriaRepository : IGenericRepository<Categorias>
    {
        private readonly string _cadenaSQL = "";

        public CategoriaRepository(IConfiguration configuration)
        {
            _cadenaSQL = configuration.GetConnectionString("cadenaSQL");

        }

        public async Task<List<Categorias>> Lista()
        {
            List<Categorias> lista= new List<Categorias>();
            using (var conexion = new SqlConnection(_cadenaSQL))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SPListaCategorias",conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using(var dr= await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Categorias
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            TipoDeCategoria = dr["TipoDeCategoria"].ToString(),
                        });
                    }
                }
            }

            return lista;
        }

        public Task<bool> Delete(int d)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(Categorias modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Guardar(Categorias modelo)
        {
            throw new NotImplementedException();
        }


    }
}
