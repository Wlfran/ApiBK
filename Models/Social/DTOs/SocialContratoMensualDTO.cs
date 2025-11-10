namespace Social_Module.Models.Social.DTOs
{
    public class SocialContratoMensualDTO
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
    }
}
