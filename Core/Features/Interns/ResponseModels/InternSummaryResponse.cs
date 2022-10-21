namespace Core.Features.Interns.ResponseModels
{
    public record InternSummaryResponse(
        Guid Id, 
        string DisplayName,  
        string Email);
}
