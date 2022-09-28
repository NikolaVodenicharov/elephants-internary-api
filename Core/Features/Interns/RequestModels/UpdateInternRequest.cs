namespace Core.Features.Interns.RequestModels
{
    public record UpdateInternRequest(
        Guid Id,
        string FirstName,
        string LastName,
        string Email);
}
