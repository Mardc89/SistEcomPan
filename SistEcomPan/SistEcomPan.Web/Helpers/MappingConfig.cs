using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
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

            config.NewConfig<Mensajes,VMMensaje>()
                .Map(dest => dest.IdMensaje, src => src.IdMensaje)
                .Map(dest => dest.Asunto, src => src.Asunto)
                .Map(dest => dest.Cuerpo, src => src.Cuerpo)
                .Ignore(dest => dest.NombreDestinatario)
                .Ignore(dest => dest.NombreRemitente)
                .Ignore(dest => dest.CorreoRemitente)
                .Ignore(dest => dest.CorreoDestinatario)
                .Ignore(dest => dest.FechaDeMensaje);
        }
    }
}
