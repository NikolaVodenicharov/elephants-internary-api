namespace WebAPI.Features.Interns.ApiRequestModels
{
    public record UpdateInternApiRequest(
        string FirstName,
        string LastName,
        string Email);
}
