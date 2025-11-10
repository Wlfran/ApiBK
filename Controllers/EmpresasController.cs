using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Module.Services.Social;

namespace Social_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly EmpresasService _empresasService;

        public EmpresasController(EmpresasService empresasService)
        {
            _empresasService = empresasService;
        }

        [HttpGet("Empresas")]
        public async Task<IActionResult> GetSEmpresas()
        {
            var data = await _empresasService.GetEmpresas();

            return Ok(new { data, totalRows = data.Count() });
        }
    }
}
