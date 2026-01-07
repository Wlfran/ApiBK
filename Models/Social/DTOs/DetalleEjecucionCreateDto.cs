namespace Social_Module.Models.Social.DTOs
{
    public class DetalleEjecucionCreateDto
    {
        public int IdSolicitud { get; set; }

        public string Nit { get; set; }
        public string NombreEmpresa { get; set; }
        public string? Localidad { get; set; }
        public string? LineaServicio { get; set; }
        public string? Otro { get; set; }
        public double ValorEjecutado { get; set; }
        public bool SinEjecucion { get; set; }
        public string CreadoPor { get; set; }
        public IFormFile? Adjunto { get; set; }


    }
}
