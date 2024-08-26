using Datos.Interfaces;
using Entidades;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Negocio.Implementacion
{
    public class DestinatarioMensajeService : IDestinatarioMensajeService
    {
        private readonly IGenericRepository<DestinatarioMensaje> _repositorio;
        private readonly IGenericRepository<Clientes> _repositorioCliente;
        private readonly IGenericRepository<Usuarios> _repositorioUsuario;
        public DestinatarioMensajeService(IGenericRepository<DestinatarioMensaje> repositorio, IGenericRepository<Clientes> repositorioCliente, IGenericRepository<Usuarios> repositorioUsuario)
        {
            _repositorio= repositorio;
            _repositorioCliente = repositorioCliente;
            _repositorioUsuario = repositorioUsuario;                                 
        }

        public async Task<string> correoDestinatario(int IdMensaje)
        {
            DestinatarioMensaje ListaDestinatarioMensaje = await _repositorio.Buscar(null, null, IdMensaje);
            var IdDestinatario = ListaDestinatarioMensaje.IdDestinatario;

            DestinatarioMensaje ListDestinatarioMensaje = await _repositorio.Buscar(null, null, IdMensaje);
            var destinatario = ListDestinatarioMensaje.Destinatario;
            string correo = "";
            if (destinatario == "Cliente")
            {
                Clientes clientes = await _repositorioCliente.Buscar(null, null, IdDestinatario);
                correo=clientes.Correo;
            }
            else if (destinatario=="Usuario")
            {
                Usuarios usuarios = await _repositorioUsuario.Buscar(null, null, IdDestinatario);
                correo=usuarios.Correo;

            }
            return correo;
            
        }

        public Task<string> Destinatario(string correo)
        {
            throw new NotImplementedException();
        }

        public Task<DestinatarioMensaje> Editar(DestinatarioMensaje entidad)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdMensaje)
        {
            throw new NotImplementedException();
        }

        public Task<int> IdDestinatario(string correo)
        {
            throw new NotImplementedException();
        }



        public async Task<List<DestinatarioMensaje>> Lista()
        {
            List<DestinatarioMensaje> ListaDestinatarioMensaje =await _repositorio.Lista();
            return ListaDestinatarioMensaje;
        }

        public async Task<string> NombreDelDestinatario(int IdMensaje)
        {
            DestinatarioMensaje ListaDestinatarioMensaje = await _repositorio.Buscar(null,null, IdMensaje);
            var IdDestinatario = ListaDestinatarioMensaje.IdDestinatario;
            var usuario = ListaDestinatarioMensaje.Destinatario;
            var nombreUsuario="";
            if (usuario == "Cliente")
            {
                Clientes clientes = await _repositorioCliente.Buscar(null, null, IdDestinatario);
                nombreUsuario = clientes.Apellidos + " " + clientes.Nombres;
            }
            else if(usuario == "Usuario")
            {
                Usuarios usuarios = await _repositorioUsuario.Buscar(null, null, IdDestinatario);
                nombreUsuario = usuarios.Apellidos + " " + usuarios.Nombres;
            }

            return nombreUsuario;
        }



        public Task<string> NombreUserDestinatario(int IdMensaje)
        {
            throw new NotImplementedException();
        }

        public Task<DestinatarioMensaje> Registrar(DestinatarioMensaje entidad)
        {
            throw new NotImplementedException();
        }


    }
}
