using Dapper;
using Microsoft.Data.SqlClient;
using Social_Module.Models.Social.DTOs;

namespace Social_Module.Services.Social
{
    public class EmpresasService
    {
        private readonly string _connectionString;

        public EmpresasService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<EmpresasDTO>> GetEmpresas()
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<EmpresasDTO>(
                "sp_DatosEmpresas",
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
    }
}
