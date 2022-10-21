namespace Core.Features.Admins.ResponseModels
{
    public record AdminSummaryResponse(
        Guid Id, 
        string DisplayName,
        string WorkEmail);
}