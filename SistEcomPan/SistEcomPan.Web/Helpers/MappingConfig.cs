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
        }
    }
}
