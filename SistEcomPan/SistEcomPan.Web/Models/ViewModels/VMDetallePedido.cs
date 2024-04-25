namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public string? DescripcionProducto { get; set; }
        public string? CategoriaProducto { get; set; }
        public int Cantidad { get; set; }
        public string? Precio { get; set; }
        public string? Total { get; set; }
    }
}
