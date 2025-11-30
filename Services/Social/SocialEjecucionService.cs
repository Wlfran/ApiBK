using Microsoft.Data.SqlClient;
using Social_Module.Models.Social.DTOs;
using Social_Module.Services.Interface;
using Dapper;

namespace Social_Module.Services.Social
{
    public class SocialEjecucionService : ISocialEjecucionService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public SocialEjecucionService(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public async Task<bool> GuardarDetalleAsync(DetalleEjecucionCreateDto dto)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string rutaAdjunto = null;

            if (dto.Adjunto != null)
            {
                var basePath = _env.WebRootPath ??
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var folder = Path.Combine(basePath, "adjuntos", "social");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}_{dto.Adjunto.FileName}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Adjunto.CopyToAsync(stream);
                }

                rutaAdjunto = $"/adjuntos/social/{fileName}";
            }

            var queryDetalle =
            @"
            INSERT INTO Social_DetalleEjecucion
            (IdSolicitud, NIT, NombreEmpresa, LineaServicio, Otro, ValorEjecutado,
             RutaAdjunto, SinEjecucion, CreadoPor, FechaRegistro)
            VALUES
            (@IdSolicitud, @Nit, @NombreEmpresa, @LineaServicio, @Otro, @ValorEjecutado,
             @RutaAdjunto, @SinEjecucion, @CreadoPor, GETDATE());
        ";

            await conn.ExecuteAsync(queryDetalle, new
            {
                dto.IdSolicitud,
                dto.Nit,
                dto.NombreEmpresa,
                dto.LineaServicio,
                dto.Otro,
                dto.ValorEjecutado,
                RutaAdjunto = rutaAdjunto,
                dto.SinEjecucion,
                dto.CreadoPor
            });

            return true;
        }

        public async Task<bool> ActualizarEstadoSolicitudAsync(int idSolicitud, string nuevoEstado)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = @"
            UPDATE Social_Solicitudes
            SET Estado = @Estado
            WHERE IdSolicitud = @IdSolicitud;
        ";

            await conn.ExecuteAsync(query, new
            {
                IdSolicitud = idSolicitud,
                Estado = nuevoEstado
            });

            return true;
        }

        public async Task<bool> GuardarHistorialAsync(HistorialAccionDto dto)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = @"
            INSERT INTO Social_HistorialAprobacion
            (IdSolicitud, FechaAccion, Accion, Estado, Comentarios, EmailUsuario)
            VALUES
            (@IdSolicitud, GETDATE(), @Accion, @Estado, @Comentarios, @EmailUsuario);
        ";

            await conn.ExecuteAsync(query, dto);
            return true;
        }

        public async Task GuardarBorradorAsync(BorradorEjecucionDto dto)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = @"
                INSERT INTO Social_BorradoresEjecucion (IdSolicitud, Usuario, JsonContenido)
                VALUES (@IdSolicitud, @Usuario, @JsonContenido);
            ";

            await conn.ExecuteAsync(query, dto);
        }

        public async Task<string?> ObtenerBorradorAsync(int idSolicitud, string usuario)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = @"
                SELECT TOP 1 JsonContenido
                FROM Social_BorradoresEjecucion
                WHERE IdSolicitud = @IdSolicitud AND Usuario = @Usuario
                ORDER BY FechaGuardado DESC;
            ";

            var json = await conn.QueryFirstOrDefaultAsync<string>(query, new
            {
                IdSolicitud = idSolicitud,
                Usuario = usuario
            });

            return json;
        }
    }

}
