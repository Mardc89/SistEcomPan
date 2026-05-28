using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
{
    public class MappingCliente:IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Clientes, VMCliente>()
                .Map(dest => dest.NombreCompleto,
                     src => (src.Apellidos + " " + src.Nombres).Trim())
                .Map(dest => dest.Estado,
                     src => src.Estado ? 1 : 0)
                .Map(dest => dest.NombreDistrito,
                     src => src.Distrito != null ? src.Distrito.NombreDistrito : "");


            config.NewConfig<VMCliente, Clientes>()
                .Map(dest => dest.Estado,
                     src => src.Estado == 1)
                .Ignore(dest => dest.Distrito)
                .Ignore(dest => dest.FechaRegistro);
        }
    }
}
