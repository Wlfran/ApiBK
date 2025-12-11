using Dapper;
using Microsoft.Data.SqlClient;
using Social_Module.Models.Social.DTOs;
using System.Data;

namespace Social_Module.Services.Social
{
    public class SocialService
    {
        private readonly string _connectionString;

        public SocialService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<SocialPendientesResponse> GetSolicitudesAsync(
            string? filter = null,
            string? empresa = null,
            string? contrato = null,
            string? areaEjecucion = null,
            int? anio = null,
            string? Mes = null,
            string? estado = null,
            string? excludeEstados = null,
            DateTime? fechaCreacion = null,
            string? sortBy = "FechaCreacion",
            string? sortDirection = "desc",
            int skip = 0,
            int take = 10,
            string? cedula = null)
        {
            using var connection = new SqlConnection(_connectionString);

            using var multi = await connection.QueryMultipleAsync(
                "sp_Social_ObtenerTodasLasSolicitudes",
                new
                {
                    Filter = filter,
                    Empresa = empresa,
                    Contrato = contrato,
                    AreaEjecucion = areaEjecucion,
                    Anio = anio,
                    Mes = Mes,
                    Estado = estado,
                    ExcludeEstados = excludeEstados,
                    FechaCreacion = fechaCreacion,
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    Skip = skip,
                    Take = take,
                    Cedula = cedula
                },
                commandType: CommandType.StoredProcedure
            );

            var data = (await multi.ReadAsync<SocialContratoMensualDTO>()).ToList();

            int total = await multi.ReadFirstOrDefaultAsync<int>();

            if (total == 0 && data.Count > 0)
                total = data.Count;

            return new SocialPendientesResponse
            {
                Data = data,
                TotalRows = total
            };
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
