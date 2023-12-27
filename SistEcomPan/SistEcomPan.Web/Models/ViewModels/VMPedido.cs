using Entidades;

namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMPedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public Clientes Cliente { get; set; }
        public string Dni { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombresCompletos { get; set; }
        public string Codigo { get; set; }
        public string MontoTotal { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }
        public List<VMDetallePedido> DetallePedido { get; set; }
    }
}
