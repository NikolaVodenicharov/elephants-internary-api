namespace Core.Features.Admins.RequestModels
{
    public record CreateAdminRepoRequest(
        string DisplayName,
        string WorkEmail
    );
}