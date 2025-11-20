using Social_Module.Models.Social.DTOs;

namespace Social_Module.Services.Interface
{
    public interface ISocialEjecucionService
    {
        Task<bool> GuardarDetalleAsync(DetalleEjecucionCreateDto dto);
        Task<bool> GuardarHistorialAsync(HistorialAccionDto dto);
        Task<bool> ActualizarEstadoSolicitudAsync(int idSolicitud, string nuevoEstado);

    }
}
