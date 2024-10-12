namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDevolucion
    {
        public int IdDevolucion { get; set; }
        public int? IdPedido { get; set; }
        public string? NombreDelProducto { get; set; }    
        public decimal? PrecioDelProducto { get; set; }
        public string? NombreDelCliente { get; set; }
        public int CantidadDevolucion { get; set; }
        public decimal Total { get; set; }
        public DateTime? FechaDeMensaje { get; set; }
    }
}
