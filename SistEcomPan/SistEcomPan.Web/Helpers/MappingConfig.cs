using Entidades;
using Mapster;
using SistEcomPan.Web.Models.ViewModels;

namespace SistEcomPan.Web.Helpers
{
    public static class MappinConfig
    {
        public static void Register(TypeAdapterConfig config)
        {
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


            ConfigPago(config);
            ConfigDetallePago(config);

        }

        private static void ConfigPago(TypeAdapterConfig config)
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





        // =====================================================
        // DETALLE PAGO
        // =====================================================
        private static void ConfigDetallePago(TypeAdapterConfig config)
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
                    FechaPago = src.FechaPago
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
                .Map(dest => dest.FechaPago, src => src.FechaPago);
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
