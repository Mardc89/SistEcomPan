﻿using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface IDistritoService
    {
        Task<List<Distritos>> lista();
        Task<IQueryable<Distritos>> ObtenerNombre();
        Task<string> ConsultarDistrito(int? IdDistrito);
    }
}
