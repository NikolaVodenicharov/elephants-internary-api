namespace WebAPI.Common
{
    public class InvitationUrlSettings
    {
        public const string SectionName = "AzureInvitationUrls";
        public string BackOfficeUrl { get; set; } = string.Empty;
        public string FrontOfficeUrl { get; set; } = string.Empty;
    }
}