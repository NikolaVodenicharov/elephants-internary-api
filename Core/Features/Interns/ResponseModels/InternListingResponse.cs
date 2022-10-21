namespace Core.Features.Interns.ResponseModels
{
    public record InternListingResponse(
        Guid Id,
        string DisplayName,
        string Email,
        IEnumerable<Guid> CampaignIds);
}
