using Dapper;
using Microsoft.Data.SqlClient;
using Social_Module.Models.Social.DTOs;
using Social_Module.Services.Interface;
using System.Data;

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
                var basePath = _config["AppSettings:BaseUrl"];

                if (string.IsNullOrWhiteSpace(basePath))
                    throw new Exception("BaseUrl no está configurado en appsettings.json");

                Directory.CreateDirectory(basePath);

                var extension = Path.GetExtension(dto.Adjunto.FileName);

                var existentes = Directory
                    .GetFiles(basePath, "adjuntoSocial-*")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .Select(name => name.Replace("adjuntoSocial-", ""))
                    .Select(n => int.TryParse(n, out var x) ? x : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                var siguiente = existentes + 1;

                var fileName = $"adjuntoSocial-{siguiente}{extension}";
                var filePath = Path.Combine(basePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Adjunto.CopyToAsync(stream);
                }

                rutaAdjunto = fileName;
            }

            var queryDetalle =
            @"
                INSERT INTO Social_DetalleEjecucion
                (IdSolicitud, NIT, NombreEmpresa, LineaServicio, Otro, ValorEjecutado,
                 RutaAdjunto, SinEjecucion, CreadoPor, FechaRegistro, Localidad)
                VALUES
                (@IdSolicitud, @Nit, @NombreEmpresa, @LineaServicio, @Otro, @ValorEjecutado,
                 @RutaAdjunto, @SinEjecucion, @CreadoPor, GETDATE(), @Localidad);
            ";

            await conn.ExecuteAsync(queryDetalle, new
            {
                dto.IdSolicitud,
                dto.Nit,
                dto.NombreEmpresa,
                dto.Localidad,
                dto.LineaServicio,
                dto.Otro,
                dto.ValorEjecutado,
                RutaAdjunto = (dto.Adjunto != null ? rutaAdjunto : null),
                dto.SinEjecucion,
                dto.CreadoPor
            });

            return true;
        }

        public async Task<bool> ActualizarEstadoSolicitudAsync(int idSolicitud, string nuevoEstado)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var estadoFinal =
                nuevoEstado.Equals("Rechazado", StringComparison.OrdinalIgnoreCase)
                ? "Pendiente por reportar"
                : nuevoEstado;

            var query = @"
            UPDATE Social_Solicitudes
            SET Estado = @Estado
            WHERE IdSolicitud = @IdSolicitud;
        ";

            await conn.ExecuteAsync(query, new
            {
                IdSolicitud = idSolicitud,
                Estado = estadoFinal
            });

            return true;
        }

        public async Task<bool> GuardarHistorialAsync(HistorialAccionDto dto)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            const string insertHistorial = @"
                INSERT INTO Social_HistorialAprobacion
                (IdSolicitud, FechaAccion, Accion, Estado, Comentarios, EmailUsuario)
                VALUES
                (@IdSolicitud, GETDATE(), @Accion, @Estado, @Comentarios, @EmailUsuario);
            ";

            await conn.ExecuteAsync(insertHistorial, dto);

            if (dto.Estado.Equals("Reportado", StringComparison.OrdinalIgnoreCase))
            {
                await conn.ExecuteAsync(
                    "NewWebContratistas_RW_Social_NotificarAprobadoresPorEjecucion",
                    new { dto.IdSolicitud },
                    commandType: CommandType.StoredProcedure
                );
            }
            else if (
                dto.Estado.Equals("Aprobado", StringComparison.OrdinalIgnoreCase) ||
                dto.Estado.Equals("Rechazado", StringComparison.OrdinalIgnoreCase) ||
                dto.Estado.Equals("Pendiente por reportar", StringComparison.OrdinalIgnoreCase)
            )
            {
                await conn.ExecuteAsync(
                    "NewWebContratistas_RW_Social_NotificarContratistasPorEjecucion",
                    new { dto.IdSolicitud },
                    commandType: CommandType.StoredProcedure
                );
            }

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

        public async Task<IEnumerable<DetalleEjecucionDto>> ObtenerDetalleAsync(int idSolicitud)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = @"
                            ;WITH UltimaFecha AS (
                                SELECT 
                                    MAX(CONVERT(DATETIME, CONVERT(CHAR(19), FechaRegistro, 120))) AS FechaReciente
                                FROM Social_DetalleEjecucion
                                WHERE IdSolicitud = @IdSolicitud
                            )
                            SELECT 
                                d.IdDetalle,
                                d.IdSolicitud,
                                d.NIT AS Nit,
                                d.NombreEmpresa,
                                d.LineaServicio,
                                d.Otro,
                                d.ValorEjecutado,
                                d.RutaAdjunto,
                                d.SinEjecucion,
                                s.EsObligatorio,
                                d.Localidad
                            FROM Social_DetalleEjecucion d
                            CROSS JOIN UltimaFecha u
                            INNER JOIN Social_Solicitudes s ON s.IdSolicitud = d.IdSolicitud
                            WHERE d.IdSolicitud = @IdSolicitud
                              AND CONVERT(DATETIME, CONVERT(CHAR(19), d.FechaRegistro, 120)) = u.FechaReciente
                            ORDER BY d.IdDetalle;
                        ";

            var datos = await conn.QueryAsync<DetalleEjecucionDto>(query, new { IdSolicitud = idSolicitud });

            string docsPublicUrl = _config["AppSettings:DocsPublicUrl"];

            foreach (var item in datos)
            {
                if (!string.IsNullOrWhiteSpace(item.RutaAdjunto))
                {
                    var fileName = Path.GetFileName(item.RutaAdjunto);

                    item.AdjuntoNombre = fileName;
                    item.AdjuntoUrl =
                        $"{docsPublicUrl}/{Uri.EscapeDataString(fileName)}";
                }
            }

            return datos;
        }




    }

}
