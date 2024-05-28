namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDetallePago
    {
        public int IdDetallePago { get; set; }
        public int IdPago { get; set; }
        public string? MontoAPagar { get; set; }
        public string? PagoDelCliente { get; set; }
        public string? DeudaDelCliente { get; set; }
        public string? CambioDelCliente{ get; set; }

    }
}
