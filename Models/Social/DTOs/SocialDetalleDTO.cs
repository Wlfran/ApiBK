namespace Social_Module.Models.Social.DTOs
{
    public class SocialDetalleDTO
    {
        public int IdSolicitud { get; set; }
        public string NumeroSolicitud { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Empresa { get; set; }
        public string NroContrato { get; set; }
        public string AreaEjecucion { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Estado { get; set; }
        public string email { get; set; }
        public DateTime fechaAccion { get; set; }
        public string accion { get; set; }
        public string comentario { get; set; }
        public string objeto { get; set; }

    }
}
