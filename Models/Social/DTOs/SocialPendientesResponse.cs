namespace Social_Module.Models.Social.DTOs
{
    public class SocialPendientesResponse
    {
        public List<SocialContratoMensualDTO> Data { get; set; } = new List<SocialContratoMensualDTO>();
        public int TotalRows { get; set; }
    }
}
