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
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository<Tokens> _repositorio;

        public TokenService(ITokenRepository<Tokens> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Tokens> Buscar(string token)
        {
            try
            {

                Tokens TokenEditar = await _repositorio.Buscar(null,token, null);

                return TokenEditar;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Crear(Tokens entidad)
        {
            try
            {
                bool respuesta = await _repositorio.Guardar(entidad);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo crear el Token");

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Tokens> Editar(Tokens entidad)
        {
            try
            {

                Tokens TokenEditar = await _repositorio.Buscar(null, null, entidad.IdToken);
                TokenEditar.Expiracion = entidad.Expiracion;
                TokenEditar.Token = entidad.Token;

                bool respuesta = await _repositorio.Editar(TokenEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar la Categoria");

                return TokenEditar;


            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
