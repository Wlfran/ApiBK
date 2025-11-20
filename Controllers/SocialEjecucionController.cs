using Microsoft.AspNetCore.Mvc;
using Social_Module.Models.Social.DTOs;
using Social_Module.Services.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Social_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialEjecucionController : ControllerBase
    {
        private readonly ISocialEjecucionService _service;

        public SocialEjecucionController(ISocialEjecucionService service)
        {
            _service = service;
        }

        [HttpPost("guardar-detalle")]
        public async Task<IActionResult> GuardarDetalle([FromForm] DetalleEjecucionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.GuardarDetalleAsync(dto);

            await _service.GuardarHistorialAsync(new HistorialAccionDto
            {
                IdSolicitud = dto.IdSolicitud,
                Accion = dto.AccionNueva ?? dto.EstadoNuevo,
                Estado = dto.EstadoNuevo,
                Comentarios = dto.Comentarios ?? "",
                EmailUsuario = dto.CreadoPor
            });

            await _service.ActualizarEstadoSolicitudAsync(dto.IdSolicitud, dto.EstadoNuevo);

            return Ok(new { ok = true });
        }


    }
}
