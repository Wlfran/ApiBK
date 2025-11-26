using Social_Module.Models.Social.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Social_Module.Services.Social
{
    public class SocialService
    {
        private readonly string _connectionString;

        public SocialService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<SocialContratoMensualDTO>> GetSolicitudesAsync(
            string? filter = null,
            string? empresa = null,
            string? contrato = null,
            string? areaEjecucion = null,
            int? anio = null,
            string? MesNombre = null,
            string? estado = null,
            string? excludeEstados = null,
            DateTime? fechaCreacion = null,
            string? sortBy = "FechaCreacion",
            string? sortDirection = "desc",
            int skip = 0,
            int take = 10)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new
            {
                Filter = filter,
                Empresa = empresa,
                Contrato = contrato,
                AreaEjecucion = areaEjecucion,
                Anio = anio,
                MesNombre = MesNombre,
                Estado = estado,
                ExcludeEstados = excludeEstados, 
                FechaCreacion = fechaCreacion,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Skip = skip,
                Take = take
            };

            return await connection.QueryAsync<SocialContratoMensualDTO>(
                "sp_Social_ObtenerTodasLasSolicitudes",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<SocialDetalleDTO>> GetSolicitudesByIdAsync(int idSolicitudes)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new
            {
                numSolicitud = idSolicitudes,
            };

            return await connection.QueryAsync<SocialDetalleDTO>(
                "sp_Social_ObtenerSolicitudPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

    }
}
