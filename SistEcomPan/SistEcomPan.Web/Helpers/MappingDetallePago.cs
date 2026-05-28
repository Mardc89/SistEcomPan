using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
{
    public class MappingDetallePago:IRegister
    {

        public void Register(TypeAdapterConfig config)
        {
            // ---------------------------------------------
            // VMDetallePago -> DetallePago
            // ---------------------------------------------
            config.NewConfig<VMDetallePago, DetallePago>()
                .MapWith(src => new DetallePago
                {
                    IdDetallePago = src.IdDetallePago,
                    IdPago = src.IdPago,
                    MontoAPagar = ToNullableDecimal(src.MontoAPagar),
                    PagoDelCliente = ToNullableDecimal(src.PagoDelCliente),
                    DeudaDelCliente = ToDecimal(src.DeudaDelCliente),
                    CambioDelCliente = ToDecimal(src.CambioDelCliente),
                    FechaDetallePago = src.FechaDetallePago
                });

            // ---------------------------------------------
            // DetallePago -> VMDetallePago
            // ---------------------------------------------
            TypeAdapterConfig<DetallePago, VMDetallePago>
                .NewConfig()
                .Map(dest => dest.IdDetallePago, src => src.IdDetallePago)
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.MontoAPagar,
                    src => src.MontoAPagar.HasValue
                        ? src.MontoAPagar.Value.ToString()
                        : "")
                .Map(dest => dest.PagoDelCliente,
                    src => src.PagoDelCliente.HasValue
                        ? src.PagoDelCliente.Value.ToString()
                        : "")
                .Map(dest => dest.DeudaDelCliente,
                    src => src.DeudaDelCliente.ToString())
                .Map(dest => dest.CambioDelCliente,
                    src => src.CambioDelCliente.ToString())
                .Map(dest => dest.FechaDetallePago, src => src.FechaDetallePago);
        }

        // =====================================================
        // HELPERS
        // =====================================================
        private static decimal ToDecimal(string? value)
        {
            return decimal.TryParse(value, out var result)
                ? result
                : 0;
        }

        private static decimal? ToNullableDecimal(string? value)
        {
            return decimal.TryParse(value, out var result)
                ? result
                : null;
        }
    }
}
