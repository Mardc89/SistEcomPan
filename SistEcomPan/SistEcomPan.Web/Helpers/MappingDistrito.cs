using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
{
    public class MappingDistrito:IRegister
    {

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Distritos, VMDistrito>();
            config.NewConfig<VMDistrito, Distritos>();
        }
    }
}
