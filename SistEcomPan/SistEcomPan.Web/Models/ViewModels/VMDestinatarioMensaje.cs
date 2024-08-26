namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMDestinatarioMensaje
    {
        public int IdMensaje { get; set; }
        public int IdDestinatario { get; set; }
        public string? Destinatario { get; set; }
        public string? NombreDestinatario { get; set; }
        public string? CorreoDestinatario { get; set; }

    }
}
