namespace Core.Features.Identity.ResponseModels
{
    public record IdentitySummaryResponse(
        string Email,
        string DisplayName
    );
}