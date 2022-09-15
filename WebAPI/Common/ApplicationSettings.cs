namespace WebAPI.Common
{
    public class ApplicationSettings
    {
        public const string SectionName = "ApplicationSettings";

        public string[] CorsAllowedOrigins { get; set; }
        public string AuthorizationUrl { get; set; }= string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ApiScope { get; set; } = string.Empty;
        public string ApiScopeDescription { get; set; } = string.Empty;
    }
}