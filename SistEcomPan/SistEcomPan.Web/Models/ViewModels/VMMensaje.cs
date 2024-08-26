namespace SistEcomPan.Web.Models.ViewModels
{
    public class VMMensaje
    {
        public int IdMensaje { get; set; }
        public int? IdRemitente { get; set; }
        public int? IdDestinatario { get; set; }
        public string? Asunto { get; set; }
        public string? Cuerpo { get; set; }
        public string? Remitente { get; set; }
        public string? Destinatario { get; set; }
        public string? NombreRemitente { get; set; }
        public string? NombreDestinatario { get; set; }
        public string? CorreoRemitente { get; set; }
        public string? CorreoDestinatario { get; set; }
        public int? IdRespuestaMensaje { get; set; }
        public DateTime? FechaDeMensaje { get; set; }
    }
}
