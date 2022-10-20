namespace Core.Features.Persons.ResponseModels
{
    public record IdentitySummaryResponse(
        string Email,
        string DisplayName
    );
}