using Entidades;

namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMPago
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public string MontoDePedido { get; set; }      
        public string Descuento { get; set; }
        public string CodigoPedido { get; set; }
        public string MontoTotalDePago { get; set; }
        public string NombreCliente { get; set; }
        public string MontoDeuda { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public List<VMDetallePago> DetallePago { get; set; }


    }
}
