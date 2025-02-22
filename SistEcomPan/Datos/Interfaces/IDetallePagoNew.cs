﻿using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Interfaces
{
    public interface IDetallePagoNew:IGenericRepository<DetallePago>
    {
        Task<List<DetallePago>> ConsultarDetallePagos(DateTime? fechaPago);
    }
}
