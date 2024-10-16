﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DetalleDevolucion
    {
        public int IdDetalleDevolucion { get; set; }
        public int IdDevolucion { get; set; }
        public Devolucion Devolucion { get; set; }
        public int IdProducto { get; set; }
        public Productos Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal? Total { get; set; }
    }
}
