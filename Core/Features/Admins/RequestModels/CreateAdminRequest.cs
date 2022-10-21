namespace Core.Features.Admins.RequestModels
{
    public record CreateAdminRequest(
        string Email,
        string ApplicationUrl
    );
}