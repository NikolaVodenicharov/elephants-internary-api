namespace Core.Features.Interns.ResponseModels
{
    public record InternSummaryResponse(
        Guid Id, 
        string FirstName, 
        string LastName, 
        string Email);
}
