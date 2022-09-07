namespace Core.Features.Campaigns.ResponseModels
{
    public record CampaignSummaryResponse(Guid Id, string Name, DateTime StartDate, DateTime EndDate, bool IsActive);
}
