namespace WebAPI.Common
{
    public class GraphSettings
    {
        public const string SectionName = "AzureAdGraph";
        public string AzureAdTenant { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = string.Empty;
    }
}