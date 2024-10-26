namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDevolucion
    {
        public int IdDevolucion { get; set; }
        public string CodigoPedido { get; set; }    
        public string CodigoDevolucion { get; set; }
        public decimal? MontoPedido { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? MontoAPagar { get; set; }
        public string? NombresCompletos { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public List<VMDetalleDevolucion> DetalleDevolucion { get; set; }
    }
}
