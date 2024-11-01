namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDashBoardCliente
    {
        public int TotalDeMisPedidos { get; set; }
        public int TotalDeMisMensajes { get; set; }
        public int TotalDeMisPagos { get; set; }

        public List<VMPagosSemana> PagosUltimaSemana { get; set; }
        public List<VMProductosSemana> ProductosTopUltimaSemana { get; set; }
    }
}
