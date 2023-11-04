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

namespace IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection services)
        {
            services.AddTransient<IPedidoRepository,PedidoRepository>();

        }
    }
}
