using Social_Module.Models.Social.DTOs;

namespace Social_Module.Services.Interface
{
    public interface ISocialEjecucionService
    {
        Task<bool> GuardarDetalleAsync(DetalleEjecucionCreateDto dto);
        Task<bool> GuardarHistorialAsync(HistorialAccionDto dto);
        Task<bool> ActualizarEstadoSolicitudAsync(int idSolicitud, string nuevoEstado);
        Task GuardarBorradorAsync(BorradorEjecucionDto dto);
        Task<string?> ObtenerBorradorAsync(int idSolicitud, string usuario);
        Task<IEnumerable<DetalleEjecucionDto>> ObtenerDetalleAsync(int idSolicitud);


    }
}
