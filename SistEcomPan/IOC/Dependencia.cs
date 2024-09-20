using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Interfaces;
using Datos.Implementacion;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Entidades;
using Negocio.Implementacion;
using Negocio.Interfaces;

namespace IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection services)
        {
            services.AddScoped<IPedidoNew, PedidoRepository>();
            services.AddScoped<IPedidoEnvio, PedidoNew>();
            services.AddScoped<IPagoNew, PagoRepository>();
            services.AddScoped<IDestinatarioNew, MensajeRepository>();
            services.AddScoped<IPagoContado, PagoNew>();
            services.AddScoped<IGenericRepository<Distritos>, DistritoRepository>();
            services.AddScoped<IGenericRepository<DestinatarioMensaje>, DestinatarioMensajeRepository>();
            services.AddScoped<IGenericRepository<Mensajes>,MensajeRepository>();
            services.AddScoped<IGenericRepository<Categorias>, CategoriaRepository>();
            services.AddScoped<IGenericRepository<Productos>, ProductoRepository>();
            services.AddScoped<IGenericRepository<Pedidos>, PedidoRepository>();
            services.AddScoped<IGenericRepository<Pagos>, PagoRepository>();
            services.AddScoped<IGenericRepository<DetallePedido>, DetallePedidoRepository>();
            services.AddScoped<IGenericRepository<DetallePago>, DetallePagoRepository>();
            services.AddScoped<IGenericRepository<NumeroDocumento>, NumDocumentoRepository>();

            services.AddScoped<IGenericRepository<Usuarios>, UsuarioRepository>();
            services.AddScoped<IGenericRepository<Configuracion>, ConfiguracionRepository>();        
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IEncriptService, EncriptService>();

            services.AddScoped<IGenericRepository<Roles>, RolRepository>();
            services.AddScoped<IGenericRepository<Clientes>, ClienteRepository>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IDistritoService, DistritoService>();
            services.AddScoped<IMensajeService, MensajeService>();
            services.AddScoped<IDestinatarioMensajeService, DestinatarioMensajeService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IDashBoardServiceCliente, DashBoardServiceCliente>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPagoService, PagoService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IDetallePedidoService, DetallePedidoService>();
            services.AddScoped<IDetallePagoService, DetallePagoService>();
            services.AddScoped<ProductoRepository>();


        }
    }
}
