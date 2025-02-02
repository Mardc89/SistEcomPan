using Datos.Interfaces;
using Microsoft.Extensions.Configuration;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Negocio.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                List<Configuracion> query = await _repositorio.Consultar("ServicioCorreo");
                Dictionary<string, string> Config = query.ToDictionary(c => c.Propiedad,c => c.Valor);

                var requiredKeys = new[] { "correo", "clave", "alias", "host", "puerto" };
                foreach (var key in requiredKeys)
                {
                    if (!Config.ContainsKey(key))
                    {
                        throw new KeyNotFoundException($"La clave de configuración '{key}' no fue encontrada.");
                    }
                }


                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(CorreoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clienteServidor.Send(correo);
                return true;

            }
            catch
            {
                return false;
            }
        }

    }
}
