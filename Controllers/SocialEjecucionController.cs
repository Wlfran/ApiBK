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

            return Ok(new { ok = true });
        }

        [HttpPost("guardar-historial")]
        public async Task<IActionResult> GuardarHistorial([FromBody] HistorialAccionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.GuardarHistorialAsync(dto);
            await _service.ActualizarEstadoSolicitudAsync(dto.IdSolicitud, dto.Estado);

            return Ok(new { ok = true });
        }


        [HttpPost("guardar-borrador")]
        public async Task<IActionResult> GuardarBorrador([FromBody] BorradorEjecucionDto dto)
        {
            try
            {
                await _service.GuardarBorradorAsync(dto);
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, error = ex.Message, stack = ex.StackTrace });
            }
        }


        [HttpGet("obtener-borrador/{idSolicitud:int}/{usuario}")]
        public async Task<IActionResult> ObtenerBorrador(int idSolicitud, string usuario)
        {
            var json = await _service.ObtenerBorradorAsync(idSolicitud, usuario);
            return Ok(new { json });
        }

        [HttpGet("detalle/{idSolicitud:int}")]
        public async Task<IActionResult> ObtenerDetalle(int idSolicitud)
        {
            var detalle = await _service.ObtenerDetalleAsync(idSolicitud);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            foreach (var item in detalle)
            {
                if (!string.IsNullOrEmpty(item.RutaAdjunto))
                {
                    item.AdjuntoUrl = $"{baseUrl}{item.RutaAdjunto}";
                    item.AdjuntoNombre = Path.GetFileName(item.RutaAdjunto);
                }
            }

            return Ok(detalle);
        }




    }
}
