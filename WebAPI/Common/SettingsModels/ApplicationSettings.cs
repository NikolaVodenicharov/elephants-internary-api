namespace WebAPI.Common.SettingsModels
{
    public class ApplicationSettings
    {
        public const string SectionName = "ApplicationSettings";

        public string[] CorsAllowedOrigins { get; set; } = null!;
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ApiScope { get; set; } = string.Empty;
        public string ApiScopeDescription { get; set; } = string.Empty;
    }
}