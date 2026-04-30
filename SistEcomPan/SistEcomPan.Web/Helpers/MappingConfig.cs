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


            // =====================================================
            // VMPago -> Pagos
            // Para Guardar / Editar
            // =====================================================
            config.NewConfig<VMPago, Pagos>()
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.IdPedido, src => src.IdPedido)
                .Map(dest => dest.MontoDePedido, src => ToDecimal(src.MontoDePedido))
                .Map(dest => dest.Descuento, src => ToDecimal(src.Descuento))
                .Map(dest => dest.MontoTotalDePago, src => ToNullableDecimal(src.MontoTotalDePago))
                .Map(dest => dest.MontoDeuda, src => ToDecimal(src.MontoDeuda))
                .Map(dest => dest.Estado, src => src.Estado)
                .Map(dest => dest.DetallePago, src => src.DetallePago);

            config.NewConfig<VMPago, Pagos>()
            .MapWith(src => new Pagos
            {
                IdPedido = src.IdPedido,
                MontoDePedido = Convert.ToDecimal(src.MontoDePedido),
                Descuento = Convert.ToDecimal(src.Descuento),
                MontoTotalDePago = Convert.ToDecimal(src.MontoTotalDePago),
                MontoDeuda = Convert.ToDecimal(src.MontoDeuda),
                Estado = src.Estado,
                DetallePago = src.DetallePago.Adapt<List<DetallePago>>()
            });


            // =====================================================
            // Pagos -> VMPago
            // Para Lista / Guardar / Editar / ObtenerMisPagos / ObtenerPagoPedido
            // =====================================================
            config.NewConfig<Pagos, VMPago>()
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.IdPedido, src => src.IdPedido)
                .Map(dest => dest.MontoDePedido, src => src.MontoDePedido.ToString())
                .Map(dest => dest.Descuento, src => src.Descuento.ToString())
                .Map(dest => dest.MontoTotalDePago, src => src.MontoTotalDePago.HasValue ? src.MontoTotalDePago.Value.ToString() : "0")
                .Map(dest => dest.MontoDeuda, src => src.MontoDeuda.ToString())
                .Map(dest => dest.FechaPago, src => src.FechaDePago)
                .Map(dest => dest.Estado, src => src.Estado)
                .Map(dest => dest.DetallePago, src => src.DetallePago)

                // Campos externos (manuales en controlador)
                .Ignore(dest => dest.NombreCliente)
                .Ignore(dest => dest.CodigoPedido)
                .Ignore(dest => dest.FechaPedido);
        }

        private static void ConfigDetallePago()
        {
            // =====================================================
            // VMDetallePago -> DetallePago
            // Para Guardar / Editar
            // =====================================================
            TypeAdapterConfig<VMDetallePago, DetallePago>
                .NewConfig()
                .Map(dest => dest.IdDetallePago, src => src.IdDetallePago)
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.MontoAPagar, src => ToNullableDecimal(src.MontoAPagar))
                .Map(dest => dest.PagoDelCliente, src => ToNullableDecimal(src.PagoDelCliente))
                .Map(dest => dest.DeudaDelCliente, src => ToDecimal(src.DeudaDelCliente))
                .Map(dest => dest.CambioDelCliente, src => ToDecimal(src.CambioDelCliente))
                .Map(dest => dest.FechaPago, src => src.FechaPago);

            // =====================================================
            // DetallePago -> VMDetallePago
            // Para ObtenerMiDetallePago / Guardar / Editar
            // =====================================================
            TypeAdapterConfig<DetallePago, VMDetallePago>
                .NewConfig()
                .Map(dest => dest.IdDetallePago, src => src.IdDetallePago)
                .Map(dest => dest.IdPago, src => src.IdPago)
                .Map(dest => dest.MontoAPagar, src => src.MontoAPagar.HasValue ? src.MontoAPagar.Value.ToString() : "")
                .Map(dest => dest.PagoDelCliente, src => src.PagoDelCliente.HasValue ? src.PagoDelCliente.Value.ToString() : "")
                .Map(dest => dest.DeudaDelCliente, src => src.DeudaDelCliente.ToString())
                .Map(dest => dest.CambioDelCliente, src => src.CambioDelCliente.ToString())
                .Map(dest => dest.FechaPago, src => src.FechaPago);

        }
    }
}
