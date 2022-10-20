namespace WebAPI.Common.SettingsModels
{
    public class AzureSettings
    {
        public const string SectionName = "AzureAD";

        public string Instance { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }
}