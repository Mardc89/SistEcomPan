namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDashBoardCliente
    {
        public int TotalDeMisPedidos { get; set; }
        public int TotalDeMisMensajes { get; set; }
        public int TotalDeMisPagos { get; set; }

        public List<VMPedidosSemana> PedidosUltimaSemana { get; set; }
        public List<VMProductosSemana> ProductosTopUltimaSemana { get; set; }
    }
}
