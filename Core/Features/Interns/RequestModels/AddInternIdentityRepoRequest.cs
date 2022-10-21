namespace Core.Features.Interns.RequestModels
{
    public record AddInternIdentityRepoRequest(
        Guid Id,
        string WorkEmail,
        string DisplayName);
}
