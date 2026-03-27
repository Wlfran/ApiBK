namespace Social_Module.Models.Social.DTOs
{
    public class DetalleEjecucionBulkDto
    {
        public int IdSolicitud { get; set; }
        public bool SinEjecucion { get; set; }
        public string CreadoPor { get; set; }
        public List<DetalleEjecucionCreateDto> Detalles { get; set; } = new();
    }
}
