using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class MappinConfig
    {
        public static void Register(TypeAdapterConfig config)
        {
            //config.NewConfig<VMRemitenteDestinatario, Mensajes>()
            //    .Map(dest => dest.Asunto, src => src.RemitenteMensaje.Asunto)
            //    .Map(dest => dest.Cuerpo, src => src.RemitenteMensaje.Cuerpo)
            //    .Map(dest => dest.IdRespuestaMensaje, src => src.RemitenteMensaje.IdRespuestaMensaje);

            //config.NewConfig<VMRemitenteDestinatario, DestinatarioMensaje>()
            //    .Map(dest => dest.IdMensaje, src => src.DestinatarioMensaje.IdMensaje);

            //config.NewConfig<VMMensaje, Mensajes>().TwoWays();

            //config.NewConfig<Mensajes, VMMensaje>()
            //    .Map(dest => dest.IdMensaje, src => src.IdMensaje)
            //    .Map(dest => dest.Remitente, src => src.Remitente)
            //    .Map(dest => dest.Cuerpo, src => src.Cuerpo)
            //    .Map(dest => dest.IdRespuestaMensaje, src => src.IdRespuestaMensaje)
            //    .Map(dest => dest.Asunto, src => src.Asunto)
            //    .Ignore(dest => dest.FechaDeMensaje);



            // =====================================================
            // VMMensaje -> Mensajes
            // Usado en:
            // Editar
            // EnviarMensajeRespuesta
            // Crear (cuando usas RemitenteMensaje)
            // =====================================================
            config.NewConfig<VMMensaje, Mensajes>()
                .IgnoreNullValues(true)

                .Map(dest => dest.IdMensaje, src => src.IdMensaje)
                .Map(dest => dest.Asunto, src => src.Asunto)
                .Map(dest => dest.Cuerpo, src => src.Cuerpo)
                .Map(dest => dest.IdRespuestaMensaje, src => src.IdRespuestaMensaje)

                // Se llenan desde servicios
                .Ignore(dest => dest.IdRemitente)
                .Ignore(dest => dest.Remitente)

                // Control backend / BD
                .Ignore(dest => dest.FechaDeMensaje)

                // Navegaciones EF
                .Ignore(dest => dest.Clientes)
                .Ignore(dest => dest.Usuarios);



            // =====================================================
            // Mensajes -> VMMensaje
            // Usado en:
            // Crear
            // Editar
            // EnviarMensajeRespuesta
            // ListaMensajes
            // ObtenerMiDetalleMensaje
            // ObtenerMisMensajes
            // ObtenerMensajeDeAsunto
            // =====================================================
            config.NewConfig<Mensajes, VMMensaje>()

                .Map(dest => dest.IdMensaje, src => src.IdMensaje)
                .Map(dest => dest.IdRemitente, src => src.IdRemitente)
                .Map(dest => dest.Asunto, src => src.Asunto)
                .Map(dest => dest.Cuerpo, src => src.Cuerpo)
                .Map(dest => dest.Remitente, src => src.Remitente)
                .Map(dest => dest.IdRespuestaMensaje, src => src.IdRespuestaMensaje)
                .Map(dest => dest.FechaDeMensaje, src => src.FechaDeMensaje)

                // No existen en Mensajes
                .Ignore(dest => dest.IdDestinatario)
                .Ignore(dest => dest.Destinatario)

                // Campos enriquecidos por servicios
                .Ignore(dest => dest.NombreRemitente)
                .Ignore(dest => dest.NombreDestinatario)
                .Ignore(dest => dest.CorreoRemitente)
                .Ignore(dest => dest.CorreoDestinatario);



            // =====================================================
            // VMDestinatarioMensaje -> DestinatarioMensaje
            // Usado en:
            // Crear
            // EnviarMensajeRespuesta
            // =====================================================
            config.NewConfig<VMDestinatarioMensaje, DestinatarioMensaje>()
                .IgnoreNullValues(true)

                .Map(dest => dest.IdMensaje, src => src.IdMensaje)

                // Se llenan desde servicios
                .Ignore(dest => dest.IdDestinatario)
                .Ignore(dest => dest.Destinatario);



            // =====================================================
            // DestinatarioMensaje -> VMDestinatarioMensaje
            // Si lo necesitas en GET futuros
            // =====================================================
            config.NewConfig<DestinatarioMensaje, VMDestinatarioMensaje>()

                .Map(dest => dest.IdMensaje, src => src.IdMensaje)
                .Map(dest => dest.IdDestinatario, src => src.IdDestinatario)
                .Map(dest => dest.Destinatario, src => src.Destinatario)

                // Campos externos
                .Ignore(dest => dest.NombreDestinatario)
                .Ignore(dest => dest.CorreoDestinatario);



            // =====================================================
            // VMRemitenteDestinatario -> Mensajes
            // Usado en Crear directo:
            // _mapper.Map<Mensajes>(modelo)
            // =====================================================
            config.NewConfig<VMRemitenteDestinatario, Mensajes>()
                .IgnoreNullValues(true)

                .Map(dest => dest.Asunto, src => src.RemitenteMensaje.Asunto)
                .Map(dest => dest.Cuerpo, src => src.RemitenteMensaje.Cuerpo)
                .Map(dest => dest.IdRespuestaMensaje, src => src.RemitenteMensaje.IdRespuestaMensaje)

                .Ignore(dest => dest.IdMensaje)
                .Ignore(dest => dest.IdRemitente)
                .Ignore(dest => dest.Remitente)
                .Ignore(dest => dest.FechaDeMensaje)
                .Ignore(dest => dest.Clientes)
                .Ignore(dest => dest.Usuarios);



            // =====================================================
            // VMRemitenteDestinatario -> DestinatarioMensaje
            // Usado en Crear directo:
            // _mapper.Map<DestinatarioMensaje>(modelo)
            // =====================================================
            config.NewConfig<VMRemitenteDestinatario, DestinatarioMensaje>()
                .IgnoreNullValues(true)

                .Ignore(dest => dest.IdMensaje)
                .Ignore(dest => dest.IdDestinatario)
                .Ignore(dest => dest.Destinatario);

        }
    }
}
