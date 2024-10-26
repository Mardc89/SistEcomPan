namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDetalleDevolucion
    {
        public int IdDetalleDevolucion { get; set; }
        public int IdDevolucion { get; set; }      
        public string? Categoria { get; set; }
        public string? Descripcion { get; set; }
        public string? Precio { get; set; }
        public int CantidadPedido { get; set; }      
        public string? Total { get; set; }
        public int CantidadDevolucion { get; set; }

 
    }
}
