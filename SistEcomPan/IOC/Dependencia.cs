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
            services.AddScoped<IGenericRepository<Categorias>, CategoriaRepository>();
            services.AddScoped<IGenericRepository<Productos>, ProductoRepository>();
            services.AddScoped<IGenericRepository<Pedidos>, PedidoRepository>();
            services.AddScoped<IGenericRepository<NumeroDocumento>, NumDocumentoRepository>();

            services.AddScoped<IGenericRepository<Usuarios>, UsuarioRepository>();
            services.AddScoped<IGenericRepository<Configuracion>, ConfiguracionRepository>();        
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IEncriptService, EncriptService>();

            services.AddScoped<IGenericRepository<Roles>, RolRepository>();
            services.AddScoped<IGenericRepository<Clientes>, ClienteRepository>();
            services.AddScoped<IRolService, RolService>();          
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();

        }
    }
}
