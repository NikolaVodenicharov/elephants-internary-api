namespace Core.Features.Interns.RequestModels
{
    public record InviteInternRequest(
        Guid Id,
        string WorkEmail, 
        string ApplicationUrl);
}
