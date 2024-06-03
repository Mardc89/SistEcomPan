namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDashBoard
    {
        public int TotalPedidos { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
        public List<VMPedidosSemana> PedidosUltimaSemana { get; set; }
        public List<VMProductosSemana> ProductosTopUltimaSemana { get; set; }
    }
}
