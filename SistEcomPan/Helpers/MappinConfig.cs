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
            config.NewConfig<VMRemitenteDestinatario, Mensajes>()
                .Map(dest => dest.Asunto, src => src.RemitenteMensaje.Asunto)
                .Map(dest => dest.Cuerpo, src => src.RemitenteMensaje.Cuerpo)
                .Map(dest => dest.IdRespuestaMensaje, src => src.RemitenteMensaje.IdRespuestaMensaje);

            config.NewConfig<VMRemitenteDestinatario, DestinatarioMensaje>()
                .Map(dest => dest.IdMensaje, src => src.DestinatarioMensaje.IdMensaje);

            config.NewConfig<VMMensaje, Mensajes>().TwoWays();

            config.NewConfig<Mensajes, VMMensaje>()
                .Map(dest => dest.IdMensaje, src => src.IdMensaje)
                .Map(dest => dest.Remitente, src => src.Remitente)
                .Map(dest => dest.Cuerpo, src => src.Cuerpo)
                .Map(dest => dest.IdRespuestaMensaje, src => src.IdRespuestaMensaje)
                .Map(dest => dest.Asunto, src => src.Asunto)
                .Ignore(dest => dest.FechaDeMensaje);


        }
    }
}
