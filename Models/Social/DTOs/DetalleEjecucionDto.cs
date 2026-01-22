namespace Social_Module.Models.Social.DTOs
{
    public class DetalleEjecucionDto
    {
        public int IdDetalle { get; set; }
        public int IdSolicitud { get; set; }
        public string Nit { get; set; }
        public string NombreEmpresa { get; set; }
        public string LineaServicio { get; set; }
        public string Otro { get; set; }
        public decimal ValorEjecutado { get; set; }
        public string? RutaAdjunto { get; set; }
        public bool SinEjecucion { get; set; }
        public bool EsObligatorio { get; set; }
        public string? AdjuntoUrl { get; set; }
        public string? AdjuntoNombre { get; set; }
        public DateTime? FechaUltimaFactura { get; set; }
        public DateTime? UltimoPagoRealizado { get; set; }
    }
}
