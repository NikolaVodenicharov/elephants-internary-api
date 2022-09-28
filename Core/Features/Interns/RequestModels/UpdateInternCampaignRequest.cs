namespace Core.Features.Interns.RequestModels
{
    public record UpdateInternCampaignRequest(Guid InternId, Guid CampaignId, Guid SpecialityId);
}
