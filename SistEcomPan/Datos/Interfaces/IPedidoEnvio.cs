﻿using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IPedidoEnvio
    {
        Task<Pedidos> Registrando(Pedidos modelo);
        Task <Pedidos>Actualizando(Pedidos entidad);
    }
}
