namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMPedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public Clientes Cliente { get; set; }
        public string Codigo { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }
        public List<DetallePedido> DetallePedido { get; set; }
    }
}
