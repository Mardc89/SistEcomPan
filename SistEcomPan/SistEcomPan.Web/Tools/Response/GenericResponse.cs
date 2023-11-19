namespace SistEcomPan.Web.Tools.Response
{
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }
        public string? Mensaje { get; set; }
        public TObject? objeto { get; set; }
        public List<TObject>? ListaOjeto { get; set; }

        
    }

}
