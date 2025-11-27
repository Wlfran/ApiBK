using Microsoft.AspNetCore.Mvc;
using Social_Module.Services.Social;

namespace Social_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        private readonly SocialService _socialService;

        public SocialController(SocialService socialService)
        {
            _socialService = socialService;
        }

        [HttpGet("Solicitudes")]
        public async Task<IActionResult> GetSolicitudes(
            string? filter = null,
            string? empresa = null,
            string? contrato = null,
            string? areaEjecucion = null,
            int? anio = null,
            string? mes = null,
            string? estado = null,
            string? excludeEstados = null,
            DateTime? fechaCreacion = null,
            string? sortBy = "FechaCreacion",
            string? sortDirection = "desc",
            int skip = 0,
            int take = 10)
        {
            var data = await _socialService.GetSolicitudesAsync(
                filter, empresa, contrato, areaEjecucion, anio, mes, estado, excludeEstados, fechaCreacion,
                sortBy, sortDirection, skip, take);

            return Ok(new { data, totalRows = data.Count() });
        }

        [HttpGet("SolicitudesById")]
        public async Task<IActionResult> GetSolicitudesById(int numSolicitud)
        {
            var data = await _socialService.GetSolicitudesByIdAsync(numSolicitud);

            return Ok(new { data, totalRows = data.Count() });
        }


    }
}
