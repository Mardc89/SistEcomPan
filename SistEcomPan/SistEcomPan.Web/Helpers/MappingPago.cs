using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
{
    public class MappingPago:IRegister
    {

        public void Register(TypeAdapterConfig config)
        {
            // ---------------------------------------------
            // VMPago -> Pagos
            // Guardar / Editar
            // ---------------------------------------------
            config.NewConfig<VMPago, Pagos>()
                .MapWith(src => new Pagos
                {
                    IdPago = src.IdPago,
                    IdPedido = src.IdPedido,
                    MontoDePedido = ToDecimal(src.MontoDePedido),
                    Descuento = ToDecimal(src.Descuento),
                    MontoTotalDePago = ToNullableDecimal(src.MontoTotalDePago),
                    MontoDeuda = ToDecimal(src.MontoDeuda),
                    Estado = src.Estado,
                    DetallePago = src.DetallePago == null
                        ? new List<DetallePago>()
                        : src.DetallePago.Adapt<List<DetallePago>>()
                });

            // ---------------------------------------------
            // Pagos -> VMPago
            // Lista / Guardar / Editar / ObtenerPagos
            // ---------------------------------------------
            TypeAdapterConfig<Pagos, VMPago>
                .NewConfig()
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.IdPedido, src => src.IdPedido)
                .Map(dest => dest.MontoDePedido, src => src.MontoDePedido.ToString())
                .Map(dest => dest.Descuento, src => src.Descuento.ToString())
                .Map(dest => dest.MontoTotalDePago,
                    src => src.MontoTotalDePago.HasValue
                        ? src.MontoTotalDePago.Value.ToString()
                        : "0")
                .Map(dest => dest.MontoDeuda, src => src.MontoDeuda.ToString())
                .Map(dest => dest.FechaPago, src => src.FechaDePago)
                .Map(dest => dest.Estado, src => src.Estado)
                .Map(dest => dest.DetallePago, src => src.DetallePago)

                // Campos externos
                .Ignore(dest => dest.NombreCliente)
                .Ignore(dest => dest.CodigoPedido)
                .Ignore(dest => dest.FechaPedido);
        }


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
