namespace Social_Module.Models.Social
{
    public class SocialSolicitud
    {
        public int IdSolicitud { get; set; }
        public string NumeroSolicitud { get; set; }
        public int IdContrato { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdAreaEjecucion { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string Comentarios { get; set; }
        public Guid CreadoPor { get; set; }
        public Guid? AprobadoPor { get; set; }
        public bool EsObligatorio { get; set; }
    }
}
